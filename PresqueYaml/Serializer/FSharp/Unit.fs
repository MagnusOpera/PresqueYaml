namespace MagnusOpera.PresqueYaml.Converters
open MagnusOpera.PresqueYaml
open System

type FSharpUnitConverter(options:YamlSerializerOptions) =
    inherit YamlConverter<obj>()

    override _.Default _ =
        Unchecked.defaultof<unit>

    override _.Read(node:YamlNode, typeToConvert:Type): obj =
        Unchecked.defaultof<unit>
