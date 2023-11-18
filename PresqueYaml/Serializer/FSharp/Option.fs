namespace MagnusOpera.PresqueYaml.Converters
open MagnusOpera.PresqueYaml

type FSharpOptionConverter<'T>() =
    inherit YamlConverter<'T option>()

    override _.Read(node:YamlNode, serializer) =
        match node with
        | YamlNode.None -> Option.None
        | _ ->
            let data = serializer.Deserialize("Option", node, typeof<'T>) :?> 'T
            Some data
