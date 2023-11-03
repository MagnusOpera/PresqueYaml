module PresqueYaml
open System
open System.Collections.Generic

[<RequireQualifiedAccess>]
type YamlNode =
    | None
    | Scalar of string
    | Sequence of string list
    | Mapping of Map<string, YamlNode>

[<RequireQualifiedAccess>]
type NodeData =
    | None
    | Scalar of string
    | Sequence of List<string>
    | Mapping of Dictionary<string, YamlNode>

[<RequireQualifiedAccess>]
type private NodeState = {
    Indent: int
    mutable Data: NodeData
}

type private LineInfo = {
    LineNum: int
    Indent: int
    Line: string
}

let parse (yamlString: string) : YamlNode =
    let createState indent =
        { NodeState.Indent = indent
          NodeState.Data = NodeData.None }

    let rec parseNode (states: NodeState list) (remainingLines: LineInfo list): (YamlNode * NodeState list * LineInfo list) =

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
            let currentState = states |> List.head
            let line = lineInfo.Line

            // DEDENT
            if lineInfo.Indent < currentState.Indent then
                dedent currentState

            else
                if currentState.Indent <> lineInfo.Indent then failwith $"Indentation error line {lineInfo.LineNum}"

                // Sequence
                if line.StartsWith("- ") then
                    let data =
                        match currentState.Data with
                        | NodeData.None ->
                            let data = List<string>()
                            currentState.Data <- data |> NodeData.Sequence
                            data
                        | NodeData.Sequence data -> data
                        | _ -> failwith $"type mismatch line {lineInfo.LineNum}"
                    let element = line.Substring(2).Trim()
                    data.Add(element)
                    parseNode states nextLineInfos

                // Mapping
                elif line.Contains(":") then
                    let sepIndex = line.IndexOf(':')
                    let key = line.Substring(0, sepIndex).Trim()
                    let value = line.Substring(sepIndex+1).Trim()

                    let value, states, nextLineInfos =
                        if String.IsNullOrEmpty value then
                            match nextLineInfos |> List.tryHead with
                            // INDENT
                            | Some nextLineInfo when lineInfo.Indent < nextLineInfo.Indent ->
                                parseNode (createState nextLineInfo.Indent :: states) nextLineInfos
                            | _ ->
                                YamlNode.None, states, nextLineInfos
                        else
                            YamlNode.Scalar value, states, nextLineInfos

                    let data =
                        match currentState.Data with
                        | NodeData.None ->
                            let data = Dictionary<string, YamlNode>()
                            currentState.Data <- data |> NodeData.Mapping
                            data
                        | NodeData.Mapping data -> data
                        | _ -> failwith $"Type mismatch line {lineInfo.LineNum}"
                    data[key] <- value
                    parseNode states nextLineInfos

                // Scalar
                else
                    match currentState.Data with
                    | NodeData.None
                    | NodeData.Scalar _ ->
                        currentState.Data <- NodeData.Scalar line
                    | _ -> failwith $"Type mismatch line {lineInfo.LineNum}"
                    parseNode states nextLineInfos

        | [] ->
            let currentState = states |> List.head
            dedent currentState

    let lines =
        yamlString.Split([| '\r'; '\n' |], StringSplitOptions.RemoveEmptyEntries)
        |> List.ofArray

    let lineInfo idx (line: string) =
        let mutable leadingSpaces = 0
        while leadingSpaces < line.Length && line[leadingSpaces] = ' ' do
            leadingSpaces <- leadingSpaces + 1
        { LineNum = idx+1; Indent = leadingSpaces; Line = line.Substring(leadingSpaces).TrimEnd() }

    let indentAndLines =
        lines
        |> List.mapi lineInfo
        |> List.filter (fun line -> line.Line.Length > 0)

    let initialState = createState 0
    let node, _, _ = parseNode [initialState] indentAndLines
    node
