module PresqueYaml.Defaults
open PresqueYaml.Serializer

let options =
    let options = YamlSerializerOptions()
    options.Converters <-
        [
            ConvertibleConverterFactory()
            NullableConverterFactory()
            ArrayConverterFactory()
            CollectionConverterFactory()
            ClassConverterFactory()
            FSharpCollectionsConverterFactory()
            FSharpOptionConverterFactory()
            FSharpRecordConverterFactory()
            FSharpUnitConverterFactory()
        ]
    options
