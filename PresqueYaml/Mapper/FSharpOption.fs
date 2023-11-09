namespace PresqueYaml.Mapper
open PresqueYaml.Model
open System

type FSharpOptionConverter<'T when 'T : null>(options:YamlSerializerOptions) =
    inherit YamlConverter<'T>()

    override _.Read(node:YamlNode, typeToConvert:Type) =
        match node with
        | YamlNode.Mapping mapping -> null
        | _ -> failwith $"Failed to convert to {typeToConvert.Name}"
