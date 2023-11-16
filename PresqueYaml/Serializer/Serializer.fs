namespace MagnusOpera.PresqueYaml
open System
open MagnusOpera.PresqueYaml

type YamlConverter() = class end

[<AbstractClass>]
type YamlConverter<'T>() =
    inherit YamlConverter()
    abstract Read: node:YamlNode * typeToConvert:Type -> 'T

    abstract Default: typeToConvert:Type -> 'T
    default _.Default _ = Unchecked.defaultof<'T>

type YamlSerializerOptions() =
    member val Converters: YamlConverterFactory list = [] with get, set
    member val NoneIsEmpty: bool = true with get, set
    member this.GetConverter(typeToConvert:Type): YamlConverterFactory =
        this.Converters
        |> List.find (fun converter -> converter.CanConvert typeToConvert)

and [<AbstractClass>] YamlConverterFactory() =
    abstract CanConvert: typeToConvert:Type -> bool
    default _.CanConvert(_) = true

    abstract CreateConverter: typeToConvert:Type * options:YamlSerializerOptions -> YamlConverter

[<AbstractClass; Sealed>]
type YamlSerializer() =

    static let getConverter (returnType:Type) (options:YamlSerializerOptions) =
        let factory = options.GetConverter returnType
        let converter = factory.CreateConverter(returnType, options)
        converter

    static member Default (returnType:Type, options:YamlSerializerOptions): obj =
        let converter = getConverter returnType options
        let defaultMethodInfo = converter.GetType() |> TypeCache.getDefault
        defaultMethodInfo.Invoke(converter, [| returnType |])

    static member Deserialize (node:YamlNode, returnType:Type, options:YamlSerializerOptions): obj =
        let converter = getConverter returnType options
        let readMethodInfo = converter.GetType() |> TypeCache.getRead
        readMethodInfo.Invoke(converter, [| node; returnType |])

    static member Deserialize<'T>(node:YamlNode, options:YamlSerializerOptions): 'T =
        YamlSerializer.Deserialize(node, typeof<'T>, options) :?> 'T

    static member Default<'T>(options:YamlSerializerOptions): 'T =
        YamlSerializer.Default(typeof<'T>, options) :?> 'T
