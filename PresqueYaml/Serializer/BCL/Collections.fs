namespace PresqueYaml.Serializer
open System.Collections.Generic
open PresqueYaml.Model
open System

type ListConverter<'T>(options:YamlSerializerOptions) =
    inherit YamlConverter<List<'T>>()

    override _.Read(node:YamlNode, typeToConvert:Type) =
        match node with
        | YamlNode.Sequence sequence ->
            sequence
            |> Seq.map (fun node -> YamlSerializer.Deserialize(node, typeof<'T>, options) :?> 'T)
            |> List
        | _ -> failwith $"Failed to convert to {typeToConvert.Name}"

type DictionaryConverter<'T>(options:YamlSerializerOptions) =
    inherit YamlConverter<Dictionary<string, 'T>>()

    override _.Read(node:YamlNode, typeToConvert:Type) =
        match node with
        | YamlNode.Mapping mapping ->
            mapping
            |> Map.map (fun key node -> YamlSerializer.Deserialize(node, typeof<'T>, options) :?> 'T)
            |> Dictionary<string, 'T>
        | _ -> failwith $"Failed to convert to {typeToConvert.Name}"

