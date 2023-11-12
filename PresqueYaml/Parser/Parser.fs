module PresqueYaml.Parser
open System
open System.Collections.Generic
open System.Text.RegularExpressions

[<RequireQualifiedAccess>]
type ScalarMode =
    | Folded // >: newline as space
    | Literal // |: newline as is

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

let (|Regex|_|) pattern input =
    let m = Regex.Match(input, pattern)
    if m.Success then Some(List.tail [ for g in m.Groups -> g.Value ])
    else None

let read (yamlString: string) : YamlNode =
    let lines = yamlString.Split("\n")

    let rec parseNode currentLineNumber currentColNumber (states: NodeState list): node:YamlNode * nextLineNumber:int =

        let parsingError msg col = failwith $"{msg} (line {currentLineNumber + 1}, column {col + 1})"

        let tryFindNoneWhitespace (line: string) startIndex stopIndex =
            let mutable pos = startIndex
            while pos < stopIndex && line[pos] = ' ' do
                pos <- pos + 1
            if pos = stopIndex then None
            else Some pos

        let dedent () =
            match states with
            | [] -> YamlNode.None, currentLineNumber
            | currentBlock :: _ ->
                match currentBlock.BlockInfo with
                | BlockInfo.Unknown ->
                    YamlNode.None, currentLineNumber
                | BlockInfo.Scalar (mode, state) ->
                    let data =
                        match mode with
                        | ScalarMode.Folded -> String.Join(' ', state)
                        | ScalarMode.Literal -> String.Join('\n', state)
                    YamlNode.Scalar data, currentLineNumber
                | BlockInfo.Sequence (state) ->
                    let data = state |> List.ofSeq
                    YamlNode.Sequence data, currentLineNumber
                | BlockInfo.Mapping (state) ->
                    let data = state |> Seq.map (|KeyValue|) |> Map
                    YamlNode.Mapping data, currentLineNumber

        // end of file ? we need to dedent
        if currentLineNumber >= lines.Length then
            dedent()
        else
            match states with
            | [] -> parsingError "Invalid parser state" 0
            | currentBlock :: parentBlocks ->
                let currentLine = lines[currentLineNumber]

                // is the line empty or comment ?
                let emptyLine = currentLine.Trim()
                if String.IsNullOrEmpty emptyLine || emptyLine.StartsWith("#") then
                    parseNode (currentLineNumber+1) currentBlock.Indent states
                else
                    // need to dedent ?
                    match tryFindNoneWhitespace currentLine 0 currentBlock.Indent with
                    | Some idx when currentBlock.Line < currentLineNumber && idx < currentBlock.Indent ->
                        // // check dedent returns to parent indentation
                        // match parentBlocks |> List.tryHead with
                        // | Some parentBlock -> if parentBlock.Indent <> idx then parsingError "Indentation error" idx
                        // | _ -> parsingError "Indentation error" idx
                        dedent()

                    | _ ->
                        let unknownBlock() =
                            match currentLine.Substring(currentColNumber) with
                            // sequence
                            | Regex "^( *)-(?:(?: +[^ ])| *$)" [spaces] ->
                                let idx = currentColNumber + spaces.Length
                                let sequenceBlock = { NodeState.Line = currentLineNumber; NodeState.Indent = idx; NodeState.BlockInfo = BlockInfo.Sequence (List<YamlNode>()) }
                                parseNode currentLineNumber idx (sequenceBlock :: parentBlocks)
                            // mapping
                            | Regex "^( *)\w+:(?:(?: +[^ ])| *$)" [spaces] ->
                                let idx = currentColNumber + spaces.Length
                                let mappingBlock = { NodeState.Line = currentLineNumber; NodeState.Indent = idx; NodeState.BlockInfo = BlockInfo.Mapping (Dictionary<string, YamlNode>()) }
                                parseNode currentLineNumber idx (mappingBlock :: parentBlocks)
                            // folded scalar
                            | Regex "^( *>)(?: *$)" [folded] ->
                                let scalarBlock = { NodeState.Line = currentLineNumber; NodeState.Indent = currentBlock.Indent; NodeState.BlockInfo = BlockInfo.Scalar (ScalarMode.Folded, List<string>()) }
                                parseNode (currentLineNumber+1) currentBlock.Indent (scalarBlock :: parentBlocks)
                            // literal scalar
                            | Regex "^( *\|)(?: *$)" [literal] ->
                                let scalarBlock = { NodeState.Line = currentLineNumber; NodeState.Indent = currentBlock.Indent; NodeState.BlockInfo = BlockInfo.Scalar (ScalarMode.Literal, List<string>()) }
                                parseNode (currentLineNumber+1) currentBlock.Indent (scalarBlock :: parentBlocks)
                            // if no content then postpone the decision
                            | Regex "^( *)$" [spaces] ->
                                parseNode (currentLineNumber+1) currentBlock.Indent states
                            // if content then folded scalar
                            | Regex "^( *)\w" [spaces] ->
                                let idx = currentColNumber + spaces.Length
                                let scalarNode = { NodeState.Line = currentLineNumber; NodeState.Indent = idx; NodeState.BlockInfo = BlockInfo.Scalar (ScalarMode.Folded, List<string>()) }
                                parseNode currentLineNumber idx (scalarNode :: parentBlocks)
                            | _ ->
                                parsingError "Unexpected content" currentColNumber

                        let scalarBlock (state: List<string>) =
                            let idx = tryFindNoneWhitespace currentLine currentColNumber currentLine.Length |> Option.defaultValue 0
                            match currentLine[idx] with
                            | '\t' -> parsingError "Unexpected tab character" idx
                            | _ ->
                                let value = currentLine.Substring(idx).TrimEnd()
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
                                let colonIndex = currentLine.IndexOf(':', currentColNumber)
                                if colonIndex = -1 then parsingError "Expecting mapping" idx
                                let key = currentLine.Substring(idx, colonIndex - idx).Trim()
                                let unknownBlock = { NodeState.Line = currentLineNumber; NodeState.Indent = currentBlock.Indent+1; NodeState.BlockInfo = BlockInfo.Unknown }
                                let value, currentLineNumber = parseNode currentLineNumber (colonIndex+1) (unknownBlock :: states)
                                state[key] <- value
                                parseNode currentLineNumber currentBlock.Indent states

                        match currentBlock.BlockInfo with
                        | BlockInfo.Unknown -> unknownBlock()
                        | BlockInfo.Scalar (_, state) -> scalarBlock state
                        | BlockInfo.Sequence (state) -> sequenceBlock state
                        | BlockInfo.Mapping (state) -> mappingBlock state

    let startBlock = { NodeState.Line = 0; NodeState.Indent = 0; NodeState.BlockInfo = BlockInfo.Unknown }
    let node, currentLineNumber = parseNode 0 0 [startBlock]
    node
