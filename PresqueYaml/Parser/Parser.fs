module PresqueYaml.Parser
open System
open System.Collections.Generic
open System.Text.RegularExpressions

[<RequireQualifiedAccess>]
type private ScalarMode =
    | Folded // >: newline as space
    | Literal // |: newline as is

[<RequireQualifiedAccess>]
type private BlockInfo =
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

    let rec parseNode (states: NodeState list) accept currentColNumber currentLineNumber : node:YamlNode * nextLineNumber:int =

        let parsingError msg col = failwith $"{msg} (line {currentLineNumber + 1}, column {col + 1})"

        let (|Regex|_|) pattern input =
            let m = Regex.Match(input, pattern)
            if m.Success then Some(List.tail [ for g in m.Groups -> g.Value ])
            else None

        let removeDoubleQuotes (s: string) =
            if String.IsNullOrEmpty s then Some s
            elif s[0] = '\'' && s.Length > 1 then
                if s[s.Length - 1] = '\'' then s.Substring(1, s.Length-2) |> Some
                else None
            elif s[0] = '\"' && s.Length > 1 then
                if s[s.Length - 1] = '\"' then s.Substring(1, s.Length-2) |> Some
                else None
            else Some s

        let removeEscapes (s: string) =
            s.Replace("\\n", "\n")
             .Replace("\\t", "\t")
             .Replace("\\r", "\r")
             .Replace("\\s", " ")

        let convertScalar (s: string) =
            s
            |> removeDoubleQuotes
            |> Option.map removeEscapes

        let dedent (states: NodeState list) nextLineNumber =
            let node =
                match states with
                | [] ->
                    parsingError "Parsing error" currentColNumber
                | currentBlock :: _ ->
                    match currentBlock.BlockInfo with
                    | BlockInfo.Unknown ->
                        YamlNode.None
                    | BlockInfo.Scalar (mode, state) ->
                        let modedData =
                            match mode with
                            | ScalarMode.Folded -> String.Join(' ', state)
                            | ScalarMode.Literal -> String.Join('\n', state)
                        let data =
                            match modedData |> convertScalar with
                            | Some data -> data
                            | _ -> parsingError "Invalid quoted string" currentBlock.Indent
                        YamlNode.Scalar data
                    | BlockInfo.Sequence state ->
                        let data = state |> List.ofSeq
                        YamlNode.Sequence data
                    | BlockInfo.Mapping state ->
                        let data = state |> Seq.map (|KeyValue|) |> Map
                        YamlNode.Mapping data
            accept node nextLineNumber

        // end of file ? we need to dedent
        if currentLineNumber >= lines.Length then
            dedent states currentLineNumber
        else
            match states with
            | [] -> parsingError "Invalid parser state" 0
            | currentBlock :: parentBlocks ->
                let currentLine = lines[currentLineNumber]

                let headline = currentLine.TrimStart()
                let leadingSpaces = currentLine.Length - headline.Length

                match headline with
                // empty line
                | "" ->
                    parseNode states accept currentBlock.Indent (currentLineNumber+1)

                // comment line
                | _ when headline[0] = '#' ->
                    parseNode states accept currentBlock.Indent (currentLineNumber+1)

                // dedent
                | _ when currentBlock.Line < currentLineNumber && leadingSpaces < currentBlock.Indent ->
                    let idx = currentLine.Length - headline.Length
                    match states |> List.tryFind (fun state -> state.Indent = idx) with
                    | None -> parsingError "Indentation error" idx
                    | _ -> dedent states currentLineNumber

                // line with content and enough chars to cover requested indent
                | _ ->
                    let blockContent = currentLine.Substring(currentColNumber)

                    let unknownBlock() =
                        match blockContent with
                        // sequence
                        | Regex "^( *)(?:-(?: | *$))" [spaces] ->
                            let idx = currentColNumber + spaces.Length
                            let sequenceBlock = { NodeState.Line = currentLineNumber
                                                  NodeState.Indent = idx
                                                  NodeState.BlockInfo = BlockInfo.Sequence (List<YamlNode>()) }
                            parseNode (sequenceBlock :: parentBlocks) accept idx currentLineNumber
                        // mapping
                        | Regex "^( *)(?:[^ ]+:(?: | *$))" [spaces] ->
                            let idx = currentColNumber + spaces.Length
                            let mappingBlock = { NodeState.Line = currentLineNumber
                                                 NodeState.Indent = idx
                                                 NodeState.BlockInfo = BlockInfo.Mapping (Dictionary<string, YamlNode>()) }
                            parseNode (mappingBlock :: parentBlocks) accept idx currentLineNumber
                        // folded scalar
                        | Regex "^( *>)(?: *$)" [_] ->
                            let scalarBlock = { NodeState.Line = currentLineNumber
                                                NodeState.Indent = currentBlock.Indent
                                                NodeState.BlockInfo = BlockInfo.Scalar (ScalarMode.Folded, List<string>()) }
                            parseNode (scalarBlock :: parentBlocks) accept currentBlock.Indent (currentLineNumber+1)
                        // literal scalar
                        | Regex "^( *\|)(?: *$)" [_] ->
                            let scalarBlock = { NodeState.Line = currentLineNumber
                                                NodeState.Indent = currentBlock.Indent
                                                NodeState.BlockInfo = BlockInfo.Scalar (ScalarMode.Literal, List<string>()) }
                            parseNode (scalarBlock :: parentBlocks) accept currentBlock.Indent (currentLineNumber+1)
                        // compact sequence
                        | Regex "^ *\[([^\]]*)\] *$" [content] ->
                            let data =
                                content.Split(',')
                                |> Array.map (fun item -> YamlNode.Scalar (item.Trim()))
                            let sequenceBlock = { NodeState.Line = currentLineNumber
                                                  NodeState.Indent = currentBlock.Indent
                                                  NodeState.BlockInfo = BlockInfo.Sequence (List<YamlNode>(data)) }
                            dedent (sequenceBlock::parentBlocks) (currentLineNumber+1)
                        | _ ->
                            // if no content then postpone the decision
                            if String.IsNullOrWhiteSpace(blockContent) then
                                parseNode states accept currentBlock.Indent (currentLineNumber+1)
                            else
                                // compact scalar
                                let scalarBlock = { NodeState.Line = currentLineNumber
                                                    NodeState.Indent = currentColNumber
                                                    NodeState.BlockInfo = BlockInfo.Scalar (ScalarMode.Folded, List<string>([])) }
                                parseNode (scalarBlock::parentBlocks) accept currentColNumber currentLineNumber

                    let scalarBlock (state: List<string>) =
                        let value, shiftIndent =
                            if state.Count > 0 then
                                blockContent, 0
                            else
                                let newValue = blockContent.TrimStart()
                                newValue, blockContent.Length - newValue.Length
                        let value = value.TrimEnd()
                        state.Add(value)
                        parseNode states accept (currentBlock.Indent+shiftIndent) (currentLineNumber+1)

                    let sequenceBlock (state: List<YamlNode>) =
                        match blockContent with
                        | Regex "^(-(?: | *$))" [spaces] ->
                            let accept value =
                                state.Add(value)
                                parseNode states accept currentBlock.Indent

                            let unknownBlock = { NodeState.Line = currentLineNumber; NodeState.Indent = currentBlock.Indent+1; NodeState.BlockInfo = BlockInfo.Unknown }
                            parseNode (unknownBlock :: states) accept (currentColNumber+spaces.Length) currentLineNumber
                        | _ ->
                            parsingError "Expecting sequence" currentBlock.Indent

                    let mappingBlock (state: Dictionary<string, YamlNode>) =
                        match blockContent with
                        | Regex "^([^ ]+:(?: | *$))" [spaces] ->
                            let accept value =
                                let key = spaces.Replace(":", "").TrimEnd()
                                state[key] <- value
                                parseNode states accept currentBlock.Indent

                            let unknownBlock = { NodeState.Line = currentLineNumber; NodeState.Indent = currentBlock.Indent+1; NodeState.BlockInfo = BlockInfo.Unknown }
                            parseNode (unknownBlock :: states) accept (currentColNumber+spaces.Length) currentLineNumber
                        | _ ->
                            parsingError "Expecting mapping" currentBlock.Indent

                    match currentBlock.BlockInfo with
                    | BlockInfo.Unknown -> unknownBlock()
                    | BlockInfo.Scalar (_, state) -> scalarBlock state
                    | BlockInfo.Sequence state -> sequenceBlock state
                    | BlockInfo.Mapping state -> mappingBlock state

    let startBlock = { NodeState.Line = 0; NodeState.Indent = 0; NodeState.BlockInfo = BlockInfo.Unknown }
    let accept value _ = value,-1
    let node, _ = parseNode [startBlock] accept 0 0
    node
