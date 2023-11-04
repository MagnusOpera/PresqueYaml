module PresqueYaml.RepresentationModel
open System
open System.Collections.Generic

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
      Indent: int
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

    let rec parseNode (states: NodeState list) (remainingLines: LineInfo list): YamlNode * NodeState list * LineInfo list =

        let dedent (state: NodeState) =
            let node =
                match state.Data with
                | NodeData.None -> YamlNode.None
                | NodeData.Scalar data ->  data |> YamlNode.Scalar
                | NodeData.Sequence data -> data |> List.ofSeq |> YamlNode.Sequence
                | NodeData.Mapping data -> data |> Seq.map (|KeyValue|) |> Map.ofSeq |> YamlNode.Mapping
            node, states |> List.tail, remainingLines

        match remainingLines with
        | lineInfo :: nextLineInfos ->
            let raiseError msg = failwith $"{msg} (line {lineInfo.LineNum})"

            let currentState = states |> List.head

            // DEDENT
            if lineInfo.Indent < currentState.Indent then
                dedent currentState

            else
                if currentState.Indent <> lineInfo.Indent then raiseError "Indentation error"
                let line = lineInfo.Line.Substring(lineInfo.Indent)

                // Sequence
                if line.StartsWith("- ") then
                    let data =
                        match currentState.Data with
                        | NodeData.None ->
                            let data = List<YamlNode>()
                            currentState.Data <- NodeData.Sequence data
                            data
                        | NodeData.Sequence data -> data
                        | _ -> raiseError "Type mismatch"

                    // extract value from descendant
                    let value, states, nextLineInfos =
                        parseNode (createState (lineInfo.Indent + 2) :: states)
                                  ({lineInfo with Indent = lineInfo.Indent + 2 } :: nextLineInfos)

                    value |> data.Add
                    parseNode states nextLineInfos

                // Mapping
                elif line.Contains(":") then
                    let sepIndex = line.IndexOf(':')
                    let key = line.Substring(0, sepIndex).TrimEnd()
                    let value = line.Substring(sepIndex+1).Trim()

                    let value, states, nextLinesInfos =
                        if String.IsNullOrEmpty value then
                            match nextLineInfos |> List.tryHead with
                            // INDENT
                            | Some nextLineInfo when lineInfo.Indent < nextLineInfo.Indent ->
                                parseNode (createState nextLineInfo.Indent :: states) nextLineInfos
                            | _ -> YamlNode.None, states, nextLineInfos
                        else
                            parseNode (createState (lineInfo.Indent + sepIndex + 1) :: states)
                                      ({lineInfo with Indent = lineInfo.Indent + sepIndex + 1} :: nextLineInfos)

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

                // Scalar
                else
                    match currentState.Data with
                    | NodeData.None -> currentState.Data <- NodeData.Scalar (line.Trim())
                    | NodeData.Scalar data -> currentState.Data <- NodeData.Scalar $"{data} {line.Trim()}"
                    | _ -> raiseError "Type mismatch"
                    parseNode states nextLineInfos

        | [] ->
            states |> List.head |> dedent

    let lines =
        yamlString.Split([| '\r'; '\n' |], StringSplitOptions.RemoveEmptyEntries)
        |> List.ofArray

    let lineInfo idx (line: string) =
        { LineInfo.LineNum = idx + 1
          LineInfo.Indent = leadingSpaces line
          LineInfo.Line = line }

    let indentAndLines =
        lines
        |> List.mapi lineInfo
        |> List.filter (fun line -> String.IsNullOrEmpty(line.Line.Trim()) |> not)

    let initialState = createState 0
    let node, _, _ = parseNode [initialState] indentAndLines
    node
