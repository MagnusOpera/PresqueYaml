namespace MagnusOpera.PresqueYaml.Converters
open MagnusOpera.PresqueYaml
open System
open Microsoft.FSharp.Core

type YamlNodeConverter() =
    inherit YamlConverter<YamlNode>()

    override _.Read(node:YamlNode, typeToConvert:Type) =
        node

type YamlNodeConverter<'T>(options:YamlSerializerOptions) =
    inherit YamlConverter<YamlNodeValue<'T>>()

    override _.Default _ = YamlNodeValue.Undefined

    override _.Read(node:YamlNode, typeToConvert:Type) =
        match node with
        | YamlNode.None ->
            YamlNodeValue<'T>.None
        | _ ->
            let data = YamlSerializer.Deserialize(node, typeof<'T>, options) :?> 'T
            YamlNodeValue<'T>.Value data
