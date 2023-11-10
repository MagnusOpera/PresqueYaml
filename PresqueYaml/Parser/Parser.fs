module PresqueYaml.Parser
open System
open System.Collections.Generic
open System.Text.RegularExpressions

[<RequireQualifiedAccess>]
type ScalarMode =
    | Folded
    | Literal
    | Inline

[<RequireQualifiedAccess>]
type BlockInfo =
    | Unknown
    | Scalar of mode:ScalarMode * state:List<string>
    | Sequence of state:List<YamlNode>
    | Mapping of state:Dictionary<string, YamlNode>

[<RequireQualifiedAccess>]
type private NodeState = {
    Line: int
    Indent: int
    BlockInfo: BlockInfo
}


let read (yamlString: string) : YamlNode =
    let lines = yamlString.Split("\n")

    let rec parseNode currentLineNumber currentColNumber (states: NodeState list): YamlNode * int =

        let parsingError msg col = failwith $"{msg} (line {currentLineNumber + 1}, column {col + 1})"

        let parseEmptyBlock() =
            let currentLine = lines[currentLineNumber]

            let charOrCommentPos = currentLine |> Seq.tryFindIndex (fun c -> c <> ' ' && c <> '#')
            match charOrCommentPos with
            | None -> parseNode (currentLineNumber+1) currentColNumber states
            | Some idx ->
                match currentLine[idx] with
                | '#' -> parseNode (currentLineNumber+1) currentColNumber states
                | '\t' -> parsingError "Unexpected tab character" idx
                | _ -> parsingError "Unexpected content" idx

        let tryFindNoneWhitespace (line: string) startIndex stopIndex =
            let mutable pos = startIndex
            while pos < stopIndex && line[pos] = ' ' do
                pos <- pos + 1
            if pos = stopIndex then None
            else Some pos

        let dedent () =
            match states with
            | [] -> YamlNode.None, currentLineNumber
            | currentBlock :: parentBlocks ->
                match currentBlock.BlockInfo with
                | BlockInfo.Unknown -> parsingError "Unexpected parsing state" 0
                | BlockInfo.Scalar (mode, state) ->
                    let data =
                        match mode with
                        | ScalarMode.Folded -> String.Join(' ', state)
                        | ScalarMode.Literal -> String.Join('\n', state)
                        | ScalarMode.Inline -> state |> Seq.exactlyOne
                    YamlNode.Scalar data, currentLineNumber
                | BlockInfo.Sequence (state) ->
                    let data = state |> List.ofSeq
                    YamlNode.Sequence data, currentLineNumber
                | BlockInfo.Mapping (state) ->
                    let data = state |> Seq.map (|KeyValue|) |> Map
                    YamlNode.Mapping data, currentLineNumber

        let indexOfAny (s: string) (startIndex: int) (searches: string list) =
            let rec indexOfAny (searches: string list) =
                match searches with
                | search :: others ->
                    match s.IndexOf(search, startIndex) with
                    | -1 -> indexOfAny others
                    | idx -> Some (search, idx)
                | _ -> None
            indexOfAny searches

        if currentLineNumber >= lines.Length then
            dedent()
        else
            match states with
            | [] -> parseEmptyBlock()
            | currentBlock :: parentBlocks ->
                let currentLine = lines[currentLineNumber]

                match tryFindNoneWhitespace currentLine 0 currentBlock.Indent with
                | Some idx when currentBlock.Line < currentLineNumber && idx < currentBlock.Indent ->
                    // check dedent returns to parent indentation
                    match parentBlocks |> List.tryHead with
                    | Some parentBlock -> if parentBlock.Indent <> idx then parsingError "Indentation error" idx
                    | _ -> ()
                    dedent()

                | _ ->
                    let unknownBlock() =
                        let marker = [ "#"; "-"; ":"; "|"; ">" ]  |> indexOfAny currentLine currentColNumber
                        match marker with
                        | None ->
                            let scalarBlock = { NodeState.Line = currentLineNumber; NodeState.Indent = currentBlock.Indent; NodeState.BlockInfo = BlockInfo.Scalar (ScalarMode.Inline, List<string>()) }
                            parseNode currentLineNumber currentColNumber (scalarBlock :: parentBlocks)
                        | Some ("#", idx) ->
                            match tryFindNoneWhitespace currentLine currentColNumber idx with
                            | None -> parseNode (currentLineNumber+1) currentBlock.Indent states
                            | Some idx -> parsingError "Unexpected content before comment" idx
                        | Some ("-", idx) ->
                            match tryFindNoneWhitespace currentLine currentColNumber idx with
                            | None ->
                                let sequenceBlock = { NodeState.Line = currentLineNumber; NodeState.Indent = idx; NodeState.BlockInfo = BlockInfo.Sequence (List<YamlNode>()) }
                                parseNode currentLineNumber idx (sequenceBlock :: parentBlocks)
                            | Some idx -> parsingError "Unexpected content before list" idx
                        | Some (":", idx) ->
                            match tryFindNoneWhitespace currentLine currentColNumber idx with
                            | None -> parsingError "Expecting key name" idx
                            | Some idx ->
                                let mappingBlock = { NodeState.Line = currentLineNumber; NodeState.Indent = idx; NodeState.BlockInfo = BlockInfo.Mapping (Dictionary<string, YamlNode>()) }
                                parseNode currentLineNumber idx (mappingBlock :: parentBlocks)
                        | Some ("|", idx) ->
                            match tryFindNoneWhitespace currentLine currentColNumber idx with
                            | None ->
                                let scalarBlock = { NodeState.Line = currentLineNumber; NodeState.Indent = idx; NodeState.BlockInfo = BlockInfo.Scalar (ScalarMode.Folded, List<string>()) }
                                parseNode (currentLineNumber+1) idx (scalarBlock :: parentBlocks)
                            | Some idx -> parsingError "Unexpected content before scalar" idx
                        | Some (">", idx) ->
                            match tryFindNoneWhitespace currentLine currentColNumber idx with
                            | None ->
                                let scalarBlock = { NodeState.Line = currentLineNumber; NodeState.Indent = idx; NodeState.BlockInfo = BlockInfo.Scalar (ScalarMode.Literal, List<string>()) }
                                parseNode (currentLineNumber+1) idx (scalarBlock :: parentBlocks)
                            | Some idx -> parsingError "Unexpected content before scalar" idx
                        | Some (_, idx) -> parsingError "Unexpected content" idx

                    let scalarBlock (mode: ScalarMode) (state: List<string>) =
                        let idx = tryFindNoneWhitespace currentLine currentColNumber currentLine.Length |> Option.defaultValue 0
                        match currentLine[idx] with
                        | '\t' -> parsingError "Unexpected tab character" idx
                        | _ ->
                            if mode = ScalarMode.Inline && state.Count > 0 then parsingError "Unexpected scalar in inline mode" idx
                            let value = currentLine.Substring(idx)
                            state.Add(value)
                            parseNode (currentLineNumber+1) currentBlock.Indent states

                    let sequenceBlock (state: List<YamlNode>) =
                        let idx = tryFindNoneWhitespace currentLine currentColNumber currentLine.Length |> Option.defaultValue 0
                        match currentLine[idx] with
                        | '\t' -> parsingError "Unexpected tab character" idx
                        | '-' ->
                            let unknownBlock = { NodeState.Line = currentLineNumber; NodeState.Indent = currentBlock.Indent+1; NodeState.BlockInfo = BlockInfo.Unknown }
                            let value, currentLineNumber = parseNode currentLineNumber (idx+1) (unknownBlock :: states)
                            state.Add(value)
                            parseNode currentLineNumber currentBlock.Indent states
                        | _ ->
                            parsingError "Expecting sequence" currentBlock.Indent

                    let mappingBlock (state: Dictionary<string, YamlNode>) =
                        let idx = tryFindNoneWhitespace currentLine currentColNumber currentLine.Length |> Option.defaultValue 0
                        match currentLine[idx] with
                        | '\t' -> parsingError "Unexpected tab character" idx
                        | _ ->
                            let colonIndex = currentLine.IndexOf(":", currentColNumber)
                            let key = currentLine.Substring(idx, colonIndex - idx).Trim()
                            let unknownBlock = { NodeState.Line = currentLineNumber; NodeState.Indent = currentBlock.Indent+1; NodeState.BlockInfo = BlockInfo.Unknown }
                            let value, currentLineNumber = parseNode currentLineNumber (colonIndex+1) (unknownBlock :: states)
                            state.Add(key, value)
                            parseNode currentLineNumber currentBlock.Indent states

                    match currentBlock.BlockInfo with
                    | BlockInfo.Unknown -> unknownBlock()
                    | BlockInfo.Scalar (mode, state) -> scalarBlock mode state
                    | BlockInfo.Sequence (state) -> sequenceBlock state
                    | BlockInfo.Mapping (state) -> mappingBlock state

    let startBlock = { NodeState.Line = 0; NodeState.Indent = 0; NodeState.BlockInfo = BlockInfo.Unknown }
    let node, currentLineNumber = parseNode 0 0 [startBlock]
    node
