namespace PresqueYaml.Serializer
open PresqueYaml
open System

type FSharpListConverter<'T>(options:YamlSerializerOptions) =
    inherit YamlConverter<list<'T>>()

    override _.Read(node:YamlNode, typeToConvert:Type) =
        match node with
        | YamlNode.None ->
            if options.NoneIsEmpty then List.empty
            else failwith $"Failed to convert None to list"
        | YamlNode.Sequence sequence ->
            sequence
            |> List.map (fun node -> YamlSerializer.Deserialize(node, typeof<'T>, options) :?> 'T)
        | _ -> failwith $"Failed to convert to {typeToConvert.Name}"

type FSharpSetConverter<'T when 'T: comparison>(options:YamlSerializerOptions) =
    inherit YamlConverter<Set<'T>>()

    override _.Read(node:YamlNode, typeToConvert:Type) =
        match node with
        | YamlNode.None ->
            if options.NoneIsEmpty then Set.empty
            else failwith $"Failed to convert None to set"
        | YamlNode.Sequence sequence ->
            sequence
            |> Seq.map (fun node -> YamlSerializer.Deserialize(node, typeof<'T>, options) :?> 'T)
            |> Set
        | _ -> failwith $"Failed to convert to {typeToConvert.Name}"

type FSharpMapConverter<'T>(options:YamlSerializerOptions) =
    inherit YamlConverter<Map<string, 'T>>()

    override _.Read(node:YamlNode, typeToConvert:Type) =
        match node with
        | YamlNode.None ->
            if options.NoneIsEmpty then Map.empty
            else failwith $"Failed to convert None to map"
        | YamlNode.Mapping mapping ->
            mapping
            |> Map.map (fun key node -> YamlSerializer.Deserialize(node, typeof<'T>, options) :?> 'T)
        | _ -> failwith $"Failed to convert to {typeToConvert.Name}"
