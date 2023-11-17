namespace MagnusOpera.PresqueYaml.Converters
open MagnusOpera.PresqueYaml
open System

type FSharpUnitConverter() =
    inherit YamlConverter<obj>()

    override _.Default (_, _) =
        Unchecked.defaultof<unit>

    override _.Read(node:YamlNode, typeToConvert:Type, _): obj =
        Unchecked.defaultof<unit>
