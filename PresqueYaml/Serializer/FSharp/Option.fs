namespace MagnusOpera.PresqueYaml.Serializer
open MagnusOpera.PresqueYaml
open System

type FSharpOptionConverter<'T>(options:YamlSerializerOptions) =
    inherit YamlConverter<'T option>()

    override _.Read(node:YamlNode, typeToConvert:Type) =
        match node with
        | YamlNode.None -> None
        | _ ->
            let data = YamlSerializer.Deserialize(node, typeof<'T>, options) :?> 'T
            Some data
