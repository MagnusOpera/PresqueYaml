namespace MagnusOpera.PresqueYaml.Converters
open MagnusOpera.PresqueYaml
open System

type YamlNodeConverter() =
    inherit YamlConverter<YamlNode>()

    override _.CanConvert(typeToConvert:Type) =
        typeToConvert = typeof<YamlNode>

    override _.Read(node:YamlNode, typeToConvert:Type) =
        node
