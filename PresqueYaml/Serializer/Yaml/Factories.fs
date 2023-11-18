namespace MagnusOpera.PresqueYaml.Converters
open System
open MagnusOpera.PresqueYaml
open TypeHelpers

type YamlNodeConverterFactory() =
    inherit YamlConverterFactory()

    override _.CanConvert (typeToConvert:Type) =
        typeToConvert = typeof<YamlNode>

    override _.CreateConverter (typeToConvert: Type) =
        YamlNodeConverter()

type YamlNodeValueConverterFactory() =
    inherit YamlConverterFactory()

    override _.CanConvert (typeToConvert:Type) =
        match getKind typeToConvert with
        | TypeKind.YamlNodeValue -> true
        | _ -> false

    override _.CreateConverter (typeToConvert: Type) =
        let converterType = typedefof<YamlNodeConverter<_>>
        converterType
            .MakeGenericType([| typeToConvert.GetGenericArguments().[0] |])
            .GetConstructor([| |])
            .Invoke([| |])
        :?> YamlConverter
