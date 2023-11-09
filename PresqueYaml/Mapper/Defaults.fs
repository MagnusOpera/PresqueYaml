module PresqueYaml.Mappers.Defaults
open PresqueYaml.Mapper

let defaultOptions =
    let options = YamlSerializerOptions()
    options.Converters <-
        [
            ConvertibleConverterFactory()
            ArrayConverterFactory()
            CollectionConverterFactory()
            FSharpConverterFactory()
        ]
    options
