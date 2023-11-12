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

    let rec parseNode (states: NodeState list) accept currentColNumber currentLineNumber : node:YamlNode * nextLineNumber:int =

        let parsingError msg col = failwith $"{msg} (line {currentLineNumber + 1}, column {col + 1})"

        let prepareScalar (s: string) =

            let s =
                if Regex.IsMatch(s, "^ *((?<![\\\\])['\"])((?:.(?!(?<![\\\\])\\1))*.?)\\1 *$") then
                    s.Trim().Substring(1, s.Length-2)
                else s

            let value =
                s.Replace("\\n", "\n")
                 .Replace("\\t", "\t")
                 .Replace("\\r", "\r")
                 .Replace("\\s", " ")
            value

        let dedent () =
            let node =
                match states with
                | [] ->
                    parsingError "Parsing error" currentColNumber
                | currentBlock :: _ ->
                    match currentBlock.BlockInfo with
                    | BlockInfo.Unknown ->
                        YamlNode.None
                    | BlockInfo.Scalar (mode, state) ->
                        let data =
                            match mode with
                            | ScalarMode.Folded -> String.Join(' ', state)
                            | ScalarMode.Literal -> String.Join('\n', state)
                        YamlNode.Scalar data
                    | BlockInfo.Sequence state ->
                        let data = state |> List.ofSeq
                        YamlNode.Sequence data
                    | BlockInfo.Mapping state ->
                        let data = state |> Seq.map (|KeyValue|) |> Map
                        YamlNode.Mapping data
            accept node currentLineNumber

        // end of file ? we need to dedent
        if currentLineNumber >= lines.Length then
            dedent()
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
                    | _ -> dedent()

                // line with content and enough chars to cover requested indent
                | _ ->
                    let blockContent = currentLine.Substring(currentColNumber)

                    let unknownBlock() =
                        match blockContent with
                        // sequence
                        | Regex "^( *)(?:-(?: | *$))" [spaces] ->
                            let idx = currentColNumber + spaces.Length
                            let sequenceBlock = { NodeState.Line = currentLineNumber; NodeState.Indent = idx; NodeState.BlockInfo = BlockInfo.Sequence (List<YamlNode>()) }
                            parseNode (sequenceBlock :: parentBlocks) accept idx currentLineNumber
                        // mapping
                        | Regex "^( *)(?:[^ ]+:(?: | *$))" [spaces] ->
                            let idx = currentColNumber + spaces.Length
                            let mappingBlock = { NodeState.Line = currentLineNumber; NodeState.Indent = idx; NodeState.BlockInfo = BlockInfo.Mapping (Dictionary<string, YamlNode>()) }
                            parseNode (mappingBlock :: parentBlocks) accept idx currentLineNumber
                        // folded scalar
                        | Regex "^( *>)(?: *$)" [_] ->
                            let scalarBlock = { NodeState.Line = currentLineNumber; NodeState.Indent = currentBlock.Indent; NodeState.BlockInfo = BlockInfo.Scalar (ScalarMode.Folded, List<string>()) }
                            parseNode (scalarBlock :: parentBlocks) accept currentBlock.Indent (currentLineNumber+1)
                        // literal scalar
                        | Regex "^( *\|)(?: *$)" [_] ->
                            let scalarBlock = { NodeState.Line = currentLineNumber; NodeState.Indent = currentBlock.Indent; NodeState.BlockInfo = BlockInfo.Scalar (ScalarMode.Literal, List<string>()) }
                            parseNode (scalarBlock :: parentBlocks) accept currentBlock.Indent (currentLineNumber+1)
                        // compact sequence
                        | Regex "^ *\[([^\]]*)\] *$" [content] ->
                            let value =
                                content.Split(',')
                                |> Seq.map (fun item -> item.Trim() |> prepareScalar |> YamlNode.Scalar)
                                |> List.ofSeq
                                |> YamlNode.Sequence
                            accept value (currentLineNumber+1)
                        | _ ->
                            // if no content then postpone the decision
                            if String.IsNullOrWhiteSpace(blockContent) then
                                parseNode states accept currentBlock.Indent (currentLineNumber+1)
                            else
                                let value = blockContent.Trim() |> prepareScalar |> YamlNode.Scalar
                                accept value (currentLineNumber+1)

                    let scalarBlock (state: List<string>) =
                        let value, shiftIndent =
                            if state.Count > 0 then
                                blockContent, 0
                            else
                                let newValue = blockContent.TrimStart()
                                newValue, blockContent.Length - newValue.Length
                        let value = value.TrimEnd() |> prepareScalar
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
