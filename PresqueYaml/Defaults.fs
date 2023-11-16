module MagnusOpera.PresqueYaml.Defaults
open MagnusOpera.PresqueYaml.Serializer

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
