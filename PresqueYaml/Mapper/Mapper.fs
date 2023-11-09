namespace PresqueYaml.Mapper
open System
open PresqueYaml.Model

type YamlConverter() =
    abstract CanConvert: typeToConvert:Type -> bool
    default _.CanConvert(_): bool = true

and YamlSerializerOptions() =
    member val Converters: YamlConverter list = [] with get, set
    member this.GetConverter(typeToConvert:Type): YamlConverter =
        this.Converters
        |> List.find (fun converter -> converter.CanConvert typeToConvert)

[<AbstractClass>]
type YamlConverter<'T>() =
    inherit YamlConverter()
    abstract Read: node:YamlNode * typeToConvert:Type -> 'T

[<AbstractClass>]
type YamlConverterFactory() =
    inherit YamlConverter()
    abstract CreateConverter: typeToConvert:Type * options:YamlSerializerOptions -> YamlConverter

[<AbstractClass; Sealed>]
type YamlSerializer() =
    static member Deserialize (node:YamlNode, returnType:Type, options:YamlSerializerOptions): obj =
        null
