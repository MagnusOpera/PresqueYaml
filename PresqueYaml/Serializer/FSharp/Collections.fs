namespace MagnusOpera.PresqueYaml.Converters
open MagnusOpera.PresqueYaml
open System

type FSharpListConverter<'T>() =
    inherit YamlConverter<list<'T>>()

    override _.Default (_, options) =
        if options.NoneIsEmpty then List.empty
        else failwith $"Failed to convert None to list"

    override _.Read(node:YamlNode, typeToConvert:Type, serializer) =
        match node with
        | YamlNode.None ->
            if serializer.Options.NoneIsEmpty then List.empty
            else failwith $"Failed to convert None to list"
        | YamlNode.Sequence sequence ->
            sequence
            |> List.map (fun node -> serializer.Deserialize("list", node, typeof<'T>) :?> 'T)
        | _ -> failwith $"Failed to convert to {typeToConvert.Name}"

type FSharpSetConverter<'T when 'T: comparison>() =
    inherit YamlConverter<Set<'T>>()

    override _.Default (_, options) =
        if options.NoneIsEmpty then Set.empty
        else failwith $"Failed to convert None to set"

    override _.Read(node:YamlNode, typeToConvert:Type, serializer) =
        match node with
        | YamlNode.None ->
            if serializer.Options.NoneIsEmpty then Set.empty
            else failwith $"Failed to convert None to set"
        | YamlNode.Sequence sequence ->
            sequence
            |> Seq.map (fun node -> serializer.Deserialize("set", node, typeof<'T>) :?> 'T)
            |> Set
        | _ -> failwith $"Failed to convert to {typeToConvert.Name}"

type FSharpMapConverter<'T>() =
    inherit YamlConverter<Map<string, 'T>>()

    override _.Default (_, options) =
        if options.NoneIsEmpty then Map.empty
        else failwith $"Failed to convert None to map"

    override _.Read(node:YamlNode, typeToConvert:Type, serializer) =
        match node with
        | YamlNode.None ->
            if serializer.Options.NoneIsEmpty then Map.empty
            else failwith $"Failed to convert None to map"
        | YamlNode.Mapping mapping ->
            mapping
            |> Map.map (fun key node -> serializer.Deserialize("map", node, typeof<'T>) :?> 'T)
        | _ -> failwith $"Failed to convert to {typeToConvert.Name}"
