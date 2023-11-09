namespace PresqueYaml.Serializer
open PresqueYaml.Model
open System

type ClassConverter<'T when 'T : null>(options:YamlSerializerOptions) =
    inherit YamlConverter<'T>()

    let classType = typeof<'T>
    let ctor = classType.GetConstructors() |> Seq.exactlyOne

    override _.Read(node:YamlNode, typeToConvert:Type) =
        match node with
        | YamlNode.Mapping mapping -> null
        | _ -> failwith $"Failed to convert to {typeToConvert.Name}"
