namespace PresqueYaml.Serializer
open PresqueYaml.Model
open System

type ConvertibleConverter<'T>(options:YamlSerializerOptions) =
    inherit YamlConverter<'T>()

    override _.Read(node:YamlNode, typeToConvert:Type) =
        match node with
        | YamlNode.Scalar data -> Convert.ChangeType(data, typeToConvert) :?> 'T
        | _ -> failwith $"Failed to convert to {typeToConvert.Name}"
