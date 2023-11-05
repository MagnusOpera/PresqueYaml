module PresqueYaml.Model
open System
open System.Collections.Generic
open System.Text.RegularExpressions

[<RequireQualifiedAccess>]
type YamlNode =
    | None
    | Scalar of string
    | Sequence of YamlNode list
    | Mapping of Map<string, YamlNode>

[<RequireQualifiedAccess>]
type private NodeData =
    | None
    | Scalar of string
    | Sequence of List<YamlNode>
    | Mapping of Dictionary<string, YamlNode>

[<RequireQualifiedAccess>]
type private NodeState =
    { Indent: int
      mutable Data: NodeData }

[<RequireQualifiedAccess>]
type private LineInfo =
    { LineNum: int
      ColNum: int
      Line: string }

let read (yamlString: string) : YamlNode =
    let createState indent =
        { NodeState.Indent = indent
          NodeState.Data = NodeData.None }

    let leadingSpaces (line: string) =
        let mutable leadingSpaces = 0
        while leadingSpaces < line.Length && line[leadingSpaces] = ' ' do
            leadingSpaces <- leadingSpaces + 1
        leadingSpaces

    let (|Regex|_|) pattern input =
        let m = Regex.Match(input, pattern)
        if m.Success then List.tail [ for g in m.Groups -> g.Value ] |> Some
        else None

    let unquoteString data =
        match data with
        | Regex "^'([^']*)'$" [content] -> content
        | Regex "^\"([^\"]*)\"$" [content] -> content
        | _ -> data.Trim()

    let toYamlNode data =
        match data with
        | NodeData.None -> YamlNode.None
        | NodeData.Scalar data ->
            match data.Trim() with
            // inline sequence
            | Regex "^\[([^\]]*)\]$" [content] ->
                content.Replace("\\n", " ").Split(',')
                |> Seq.map (fun item ->
                    if String.IsNullOrWhiteSpace(item) then YamlNode.None
                    else item |> unquoteString |> YamlNode.Scalar)
                |> List.ofSeq
                |> YamlNode.Sequence
            // single quoted string
            | Regex "^'([^']*)'$" [content] -> content.Replace("\\n", "\n") |> YamlNode.Scalar
            // double quoted string
            | Regex "^\"([^\"]*)\"$" [content] -> content.Replace("\\n", "\n") |> YamlNode.Scalar
            // unquoted string
            | data -> data.Replace("\\n", "\n") |> YamlNode.Scalar
        | NodeData.Sequence data -> data |> List.ofSeq |> YamlNode.Sequence
        | NodeData.Mapping data -> data |> Seq.map (|KeyValue|) |> Map.ofSeq |> YamlNode.Mapping

    let rec parseNode (states: NodeState list) (remainingLines: LineInfo list): YamlNode * NodeState list * LineInfo list =

        let dedent (state: NodeState) =
            let node = state.Data |> toYamlNode
            node, states |> List.tail, remainingLines

        match remainingLines with
        | lineInfo :: nextLineInfos ->
            let raiseError msg = failwith $"{msg} (line {lineInfo.LineNum + 1}, column {lineInfo.ColNum + 1})"

            let currentState = states |> List.head

            let sequence () =
                let data =
                    match currentState.Data with
                    | NodeData.None ->
                        let data = List<YamlNode>()
                        currentState.Data <- NodeData.Sequence data
                        data
                    | NodeData.Sequence data -> data
                    | _ -> raiseError "Type mismatch"

                let value, states, nextLineInfos =
                    parseNode (createState (lineInfo.ColNum + 2) :: states)
                              ({lineInfo with ColNum = lineInfo.ColNum + 2 } :: nextLineInfos)

                value |> data.Add
                parseNode states nextLineInfos

            let mapping (line: string) =
                let sepIndex = line.IndexOf(':')
                let key = line.Substring(0, sepIndex).TrimEnd() |> unquoteString
                let value = line.Substring(sepIndex+1)

                let value, states, nextLinesInfos =
                    if String.IsNullOrWhiteSpace value then
                        match nextLineInfos |> List.tryHead with
                        // INDENT
                        | Some nextLineInfo when lineInfo.ColNum < nextLineInfo.ColNum ->
                            parseNode (createState nextLineInfo.ColNum :: states) nextLineInfos
                        | _ -> YamlNode.None, states, nextLineInfos
                    else
                        let leadingSpaces = leadingSpaces value
                        parseNode (createState (lineInfo.ColNum + sepIndex + 1 + leadingSpaces) :: states)
                                  ({lineInfo with ColNum = lineInfo.ColNum + sepIndex + 1 + leadingSpaces} :: nextLineInfos)

                let data =
                    match currentState.Data with
                    | NodeData.None ->
                        let data = Dictionary<string, YamlNode>()
                        currentState.Data <- NodeData.Mapping data
                        data
                    | NodeData.Mapping data -> data
                    | _ -> raiseError "Type mismatch"
                data[key] <- value
                parseNode states nextLinesInfos

            let scalar (line: string) =
                match currentState.Data with
                | NodeData.None -> currentState.Data <- NodeData.Scalar (line.Trim())
                | NodeData.Scalar data -> currentState.Data <- NodeData.Scalar $"{data}\n{line.Trim()}"
                | _ -> raiseError "Type mismatch"
                parseNode states nextLineInfos

            // DEDENT
            if lineInfo.ColNum < currentState.Indent then
                dedent currentState
            else
                if currentState.Indent <> lineInfo.ColNum then raiseError "Indentation error"
                let line = lineInfo.Line.Substring(lineInfo.ColNum)

                if line.StartsWith("- ") then sequence ()
                elif line.Contains(":") then mapping line
                else scalar line

        | [] ->
            states |> List.head |> dedent

    let lines =
        yamlString.Split([| '\r'; '\n' |], StringSplitOptions.RemoveEmptyEntries)
        |> List.ofArray

    let lineInfo idx (line: string) =
        { LineInfo.LineNum = idx
          LineInfo.ColNum = leadingSpaces line
          LineInfo.Line = line }

    let isCommentOrEmpty (lineInfo: LineInfo) =
        lineInfo.Line |> String.IsNullOrWhiteSpace || lineInfo.Line.TrimStart()[0] = '#'

    let node, _, _ =
        lines
        |> List.mapi lineInfo
        |> List.filter (not << isCommentOrEmpty)
        |> parseNode [createState 0]
    node
