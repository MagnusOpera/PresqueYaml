namespace PresqueYaml.Serializer
open PresqueYaml
open System

type FSharpUnitConverter(options:YamlSerializerOptions) =
    inherit YamlConverter<obj>()

    override _.Read(node:YamlNode, typeToConvert:Type): obj =
        null
