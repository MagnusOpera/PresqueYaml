namespace MagnusOpera.PresqueYaml.Converters
open MagnusOpera.PresqueYaml


[<Sealed>]
type FSharpUnitConverter() =
    inherit YamlConverter<obj>()

    override _.Read(node:YamlNode, _): obj =
        Unchecked.defaultof<unit>
