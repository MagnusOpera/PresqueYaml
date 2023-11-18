namespace MagnusOpera.PresqueYaml.Converters
open MagnusOpera.PresqueYaml

type FSharpUnitConverter() =
    inherit YamlConverter<obj>()

    override _.Default _ =
        Unchecked.defaultof<unit>

    override _.Read(node:YamlNode, _): obj =
        Unchecked.defaultof<unit>
