namespace MagnusOpera.PresqueYaml.Converters
open MagnusOpera.PresqueYaml


[<Sealed>]
type YamlNodeConverter() =
    inherit YamlConverter<YamlNode>()

    override _.Read(node:YamlNode, _) =
        node


[<Sealed>]
type YamlNodeConverter<'T>() =
    inherit YamlConverter<YamlNodeValue<'T>>()

    override _.Default _ =
        YamlNodeValue<'T>.Undefined

    override _.Read(node:YamlNode, serializer) =
        match node with
        | YamlNode.None ->
            YamlNodeValue<'T>.None
        | _ ->
            let data = serializer.Deserialize("YamlNode", node, typeof<'T>) :?> 'T
            YamlNodeValue<'T>.Value data
