namespace MagnusOpera.PresqueYaml.Converters
open MagnusOpera.PresqueYaml


[<Sealed>]
type FSharpListConverter<'T>() =
    inherit YamlConverter<'T list>()

    override _.Default options =
        if options.NoneIsEmpty then List.empty
        else Unchecked.defaultof<'T list>

    override _.Read(node:YamlNode, serializer) =
        match node with
        | YamlNode.None ->
            if serializer.Options.NoneIsEmpty then List.empty
            else failwith $"Failed to convert None to list"
        | YamlNode.Sequence sequence ->
            sequence
            |> List.map (fun node -> serializer.Deserialize("list", node, typeof<'T>) :?> 'T)
        | _ -> failwith $"Failed to convert to {typeof<list<'T>>.Name}"


[<Sealed>]
type FSharpSetConverter<'T when 'T: comparison>() =
    inherit YamlConverter<Set<'T>>()

    override _.Default options =
        if options.NoneIsEmpty then Set.empty
        else Unchecked.defaultof<Set<'T>>

    override _.Read(node:YamlNode, serializer) =
        match node with
        | YamlNode.None ->
            if serializer.Options.NoneIsEmpty then Set.empty
            else failwith $"Failed to convert None to set"
        | YamlNode.Sequence sequence ->
            sequence
            |> Seq.map (fun node -> serializer.Deserialize("set", node, typeof<'T>) :?> 'T)
            |> Set
        | _ -> failwith $"Failed to convert to {typeof<Set<'T>>.Name}"


[<Sealed>]
type FSharpMapConverter<'T>() =
    inherit YamlConverter<Map<string, 'T>>()

    override _.Default options =
        if options.NoneIsEmpty then Map.empty
        else Unchecked.defaultof<Map<string, 'T>>

    override _.Read(node:YamlNode, serializer) =
        match node with
        | YamlNode.None ->
            if serializer.Options.NoneIsEmpty then Map.empty
            else failwith $"Failed to convert None to map"
        | YamlNode.Mapping mapping ->
            mapping
            |> Map.map (fun key node -> serializer.Deserialize("map", node, typeof<'T>) :?> 'T)
        | _ -> failwith $"Failed to convert to {typeof<Map<string, 'T>>.Name}"
