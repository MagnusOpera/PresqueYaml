module PresqueYaml
open System
open System.Collections.Generic

[<RequireQualifiedAccess>]
type YamlNode =
    | None
    | Scalar of string
    | Sequence of string list
    | Mapping of Map<string, YamlNode>

type private YamlState = {
    Id: Guid
    Indent: int
    mutable List: List<string>
    mutable Mapping: Dictionary<string, YamlNode>
}

let parse (yamlString: string) : YamlNode =
    let lines =
        yamlString.Split([| '\r'; '\n' |], StringSplitOptions.RemoveEmptyEntries)
        |> List.ofArray

    let lineInfo (line: string) =
        let mutable leadingSpaces = 0
        while leadingSpaces < line.Length && line[leadingSpaces] = ' ' do
            leadingSpaces <- leadingSpaces + 1
        leadingSpaces, line.Substring(leadingSpaces).TrimEnd()

    let rec parseNode states lineNum (remainingLines: string list) =
        let currentState = states |> List.head

        let dedent () =
            let node =
                if currentState.List |> isNull |> not then
                    currentState.List |> List.ofSeq |> YamlNode.Sequence
                elif currentState.Mapping |> isNull |> not then
                    currentState.Mapping |> Seq.map (|KeyValue|) |> Map.ofSeq |> YamlNode.Mapping
                else
                    YamlNode.None
            node, states |> List.tail, remainingLines

        match remainingLines with
        | line :: lines ->
            let leadingSpaces, line = lineInfo line

            // ignore empty lines
            if String.IsNullOrWhiteSpace line then
                parseNode states (lineNum+1) lines

            // INDENT
            elif currentState.Indent < leadingSpaces then
                let newState = { Id = Guid.NewGuid(); Indent = leadingSpaces; List = null; Mapping = null }
                parseNode (newState::states) lineNum remainingLines

            // DEDENT
            elif leadingSpaces < currentState.Indent then
                dedent()

            // Mapping
            elif line.Contains(":") then
                let sepIndex = line.IndexOf(':')
                let key = line.Substring(0, sepIndex).Trim()
                let value = line.Substring(sepIndex+1).Trim()

                let value, _, lines =
                    if String.IsNullOrEmpty value then
                        parseNode states (lineNum+1) lines
                    else
                        YamlNode.Scalar value, states, lines

                if currentState.List |> isNull |> not then failwith $"Expecting sequence line {lineNum}"
                if currentState.Mapping |> isNull then currentState.Mapping <- Dictionary<string, YamlNode>()
                currentState.Mapping[key] <- value
                parseNode states (lineNum+1) lines

            // Sequence
            elif line.StartsWith("- ") then
                if currentState.Mapping |> isNull |> not then failwith $"Expecting mapping line {lineNum}"
                if currentState.List |> isNull then currentState.List <- List<string>()
                let element = line.Substring(2).Trim()
                currentState.List.Add(element)
                parseNode states (lineNum+1) lines

            // Scalar
            else
                if currentState.Mapping |> isNull |> not then failwith $"Expecting mapping line {lineNum}"
                if currentState.List |> isNull |> not then failwith $"Expecting sequence line {lineNum}"
                YamlNode.Scalar line, states, lines

        | _ ->
            dedent()

    let initialState = { Id = Guid.NewGuid(); Indent = 0; List = null; Mapping = null }
    let node, _, remainingLines = parseNode [initialState] 1 lines
    let lineNum = (lines |> List.length) - (remainingLines |> List.length) + 1
    if remainingLines <> List.empty then failwith $"Unexpected elements line {lineNum}"
    node
