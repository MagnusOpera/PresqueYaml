namespace MagnusOpera.PresqueYaml
open System
open MagnusOpera.PresqueYaml


type YamlSerializerOptions() =
    member val Converters: YamlConverterFactory list = [] with get, set
    member val NoneIsEmpty: bool = true with get, set
    member this.GetConverter(typeToConvert:Type): YamlConverterFactory =
        this.Converters
        |> List.find (fun converter -> converter.CanConvert typeToConvert)

and [<AbstractClass>] YamlConverterFactory() =
    abstract CanConvert: typeToConvert:Type -> bool
    default _.CanConvert(_) = true

    abstract CreateConverter: typeToConvert:Type -> YamlConverter

and IYamlSerializer =
    abstract member Options: YamlSerializerOptions
    abstract member Default: returnType:Type -> obj
    abstract member Deserialize: context:string * node:YamlNode * returnType:Type -> obj
    abstract member Deserialize: context:string * node:YamlNode -> 'T

and YamlConverter() = class end

and [<AbstractClass>] YamlConverter<'T>() =
    inherit YamlConverter()
    abstract Read: node:YamlNode * serializer:IYamlSerializer -> 'T

    abstract Default: options:YamlSerializerOptions -> obj
    default _.Default (options:YamlSerializerOptions): obj = null
