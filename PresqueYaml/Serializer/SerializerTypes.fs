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
    abstract member Default: returnType:Type -> objnull
    abstract member Deserialize: context:string * node:YamlNode * returnType:Type -> objnull

and [<AbstractClass>] YamlConverter<'T>() =
    inherit YamlConverter()
    abstract Read: node:YamlNode * options:YamlSerializerOptions * serializer:IYamlSerializer -> 'T

    abstract Default: options:YamlSerializerOptions -> 'T
    default _.Default (options:YamlSerializerOptions) = Unchecked.defaultof<'T>


type YamlSerializerException(msg:string, innerEx: Exception | null) =
    inherit Exception(msg, innerEx)

    static member Raise(msg, ?innerEx: Exception) =
        let innerEx: Exception | null =
            match innerEx with
            | None -> null
            | Some ex -> ex
        YamlSerializerException(msg, innerEx)
        |> raise
