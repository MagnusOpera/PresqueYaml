module PresqueYaml.Mappers.Defaults
open PresqueYaml.Mapper

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
