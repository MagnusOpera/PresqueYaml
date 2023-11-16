namespace MagnusOpera.PresqueYaml.Serializer
open MagnusOpera.PresqueYaml
open System

type FSharpUnitConverter(options:YamlSerializerOptions) =
    inherit YamlConverter<obj>()

    override _.Read(node:YamlNode, typeToConvert:Type): obj =
        Unchecked.defaultof<unit>
