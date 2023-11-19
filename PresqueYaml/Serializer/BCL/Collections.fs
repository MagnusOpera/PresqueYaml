namespace MagnusOpera.PresqueYaml.Converters
open System.Collections.Generic
open MagnusOpera.PresqueYaml


[<Sealed>]
type ListConverter<'T>() =
    inherit YamlConverter<List<'T>>()

    override _.Default options =
        if options.NoneIsEmpty then List<'T>()
        else null

    override _.Read(node, options, serializer) =
        match node with
        | YamlNode.None ->
            if options.NoneIsEmpty then List<'T>()
            else failwith $"Failed to convert None to list"
        | YamlNode.Sequence sequence ->
            sequence
            |> Seq.map (fun node -> serializer.Deserialize("List", node, typeof<'T>) :?> 'T)
            |> List
        | _ -> failwith $"Failed to convert to {typeof<List<'T>>.Name}"


[<Sealed>]
type DictionaryConverter<'T>() =
    inherit YamlConverter<Dictionary<string, 'T>>()

    override _.Default options =
        if options.NoneIsEmpty then Dictionary<string, 'T>()
        else null

    override _.Read(node, options, serializer) =
        match node with
        | YamlNode.None ->
            if options.NoneIsEmpty then Dictionary<string, 'T>()
            else failwith $"Failed to convert None to dictionary"
        | YamlNode.Mapping mapping ->
            mapping
            |> Map.map (fun key node -> serializer.Deserialize("Dictionary", node, typeof<'T>) :?> 'T)
            |> Dictionary<string, 'T>
        | _ -> failwith $"Failed to convert to {typeof<Dictionary<string, 'T>>.Name}"


[<Sealed>]
type ArrayConverter<'T>() =
    inherit YamlConverter<'T[]>()

    override _.Default options =
        if options.NoneIsEmpty then Array.empty
        else null

    override _.Read(node, options, serializer) =
        match node with
        | YamlNode.None ->
            if options.NoneIsEmpty then Array.empty
            else failwith $"Failed to convert None to array"
        | YamlNode.Sequence sequence ->
            sequence
            |> Seq.map (fun node -> serializer.Deserialize("Array", node, typeof<'T>) :?> 'T)
            |> Array.ofSeq
        | _ -> failwith $"Failed to convert to {typeof<'T[]>.Name}"
