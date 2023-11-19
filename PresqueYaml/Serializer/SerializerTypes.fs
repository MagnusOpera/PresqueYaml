namespace MagnusOpera.PresqueYaml
open System
open MagnusOpera.PresqueYaml


[<AbstractClass>]
type YamlConverter() = class end


[<Sealed>]
type YamlSerializerOptions() =
    member val Converters: YamlConverterFactory list = [] with get, set
    member val NoneIsEmpty: bool = true with get, set

and [<AbstractClass>] YamlConverterFactory() =
    abstract CanConvert: typeToConvert:Type -> bool
    abstract CreateConverter: typeToConvert:Type * options:YamlSerializerOptions -> YamlConverter


type IYamlSerializer =
    abstract member Default: returnType:Type -> obj
    abstract member Deserialize: context:string * node:YamlNode * returnType:Type -> obj

and [<AbstractClass>] YamlConverter<'T>() =
    inherit YamlConverter()
    abstract Read: node:YamlNode * options:YamlSerializerOptions * serializer:IYamlSerializer -> 'T

    abstract Default: options:YamlSerializerOptions -> 'T
    default _.Default (options:YamlSerializerOptions) = Unchecked.defaultof<'T>


type YamlSerializerException(msg:string, ?innerEx:Exception) =
    inherit Exception(msg, innerEx |> Option.defaultValue null)

    static member Raise(msg) = YamlSerializerException(msg) |> raise
