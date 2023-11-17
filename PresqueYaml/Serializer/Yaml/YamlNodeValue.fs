namespace MagnusOpera.PresqueYaml.Converters
open MagnusOpera.PresqueYaml
open System
open Microsoft.FSharp.Core

type YamlNodeConverter() =
    inherit YamlConverter<YamlNode>()

    override _.Read(node:YamlNode, typeToConvert:Type, _) =
        node

type YamlNodeConverter<'T>() =
    inherit YamlConverter<YamlNodeValue<'T>>()

    override _.Default (_, _) =
        YamlNodeValue.Undefined

    override _.Read(node:YamlNode, typeToConvert:Type, serializer) =
        match node with
        | YamlNode.None ->
            YamlNodeValue<'T>.None
        | _ ->
            let data = serializer.Deserialize("YamlNode", node, typeof<'T>) :?> 'T
            YamlNodeValue<'T>.Value data
