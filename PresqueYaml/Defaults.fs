module MagnusOpera.PresqueYaml.Defaults
open MagnusOpera.PresqueYaml.Converters

let options =
    let options = YamlSerializerOptions()
    options.Converters <-
        [
            YamlNodeConverterFactory()
            YamlNodeValueConverterFactory()
            YamlNodeValueConverterFactory()
            ConvertibleConverterFactory()
            NullableConverterFactory()
            ArrayConverterFactory()
            CollectionConverterFactory()
            ClassConverterFactory()
            FSharpCollectionsConverterFactory()
            FSharpUnionConverterFactory()
            FSharpRecordConverterFactory()
            FSharpUnitConverterFactory()
        ]
    options
