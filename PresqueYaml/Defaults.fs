module MagnusOpera.PresqueYaml.Defaults
open MagnusOpera.PresqueYaml.Converters

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
