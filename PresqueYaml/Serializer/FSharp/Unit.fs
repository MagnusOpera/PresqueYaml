namespace MagnusOpera.PresqueYaml.Converters
open MagnusOpera.PresqueYaml


[<Sealed>]
type FSharpUnitConverter() =
    inherit YamlConverter<obj>()

    override _.Read(node, _, _): obj =
        Unchecked.defaultof<unit>
