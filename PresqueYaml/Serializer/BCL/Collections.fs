namespace PresqueYaml.Serializer
open System.Collections.Generic
open PresqueYaml
open System

type ListConverter<'T>(options:YamlSerializerOptions) =
    inherit YamlConverter<List<'T>>()

    override _.Read(node:YamlNode, typeToConvert:Type) =
        match node with
        | YamlNode.None -> List<'T>()
        | YamlNode.Sequence sequence ->
            sequence
            |> Seq.map (fun node -> YamlSerializer.Deserialize(node, typeof<'T>, options) :?> 'T)
            |> List
        | _ -> failwith $"Failed to convert to {typeToConvert.Name}"

type DictionaryConverter<'T>(options:YamlSerializerOptions) =
    inherit YamlConverter<Dictionary<string, 'T>>()

    override _.Read(node:YamlNode, typeToConvert:Type) =
        match node with
        | YamlNode.None -> Dictionary<string, 'T>()
        | YamlNode.Mapping mapping ->
            mapping
            |> Map.map (fun key node -> YamlSerializer.Deserialize(node, typeof<'T>, options) :?> 'T)
            |> Dictionary<string, 'T>
        | _ -> failwith $"Failed to convert to {typeToConvert.Name}"

type ArrayConverter<'T when 'T: comparison>(options:YamlSerializerOptions) =
    inherit YamlConverter<'T[]>()

    override _.Read(node:YamlNode, typeToConvert:Type) =
        match node with
        | YamlNode.None -> Array.empty
        | YamlNode.Sequence sequence ->
            sequence
            |> Seq.map (fun node -> YamlSerializer.Deserialize(node, typeof<'T>, options) :?> 'T)
            |> Array.ofSeq
        | _ -> failwith $"Failed to convert to {typeToConvert.Name}"
