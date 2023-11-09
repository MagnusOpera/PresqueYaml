module PresqueYaml.Mappers.Defaults
open PresqueYaml.Serializer

let defaultOptions =
    let options = YamlSerializerOptions()
    options.Converters <-
        [
            ConvertibleConverterFactory()
            NullableConverterFactory()
            ArrayConverterFactory()
            CollectionConverterFactory()
            FSharpCollectionsConverterFactory()
            FSharpOptionConverterFactory()
            FSharpRecordConverterFactory()
        ]
    options
