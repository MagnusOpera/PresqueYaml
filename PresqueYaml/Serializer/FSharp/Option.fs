namespace MagnusOpera.PresqueYaml.Converters
open MagnusOpera.PresqueYaml
open System

type FSharpOptionConverter<'T>() =
    inherit YamlConverter<'T option>()

    override _.Read(node:YamlNode, typeToConvert:Type, serializer) =
        match node with
        | YamlNode.None -> Option.None
        | _ ->
            let data = serializer.Deserialize("Option", node, typeof<'T>) :?> 'T
            Some data
