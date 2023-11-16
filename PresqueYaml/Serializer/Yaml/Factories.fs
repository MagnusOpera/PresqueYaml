namespace MagnusOpera.PresqueYaml.Converters
open System
open MagnusOpera.PresqueYaml

type YamlNodeConverterFactory() =
    inherit YamlConverterFactory()

    override _.CanConvert (typeToConvert:Type) =
        typeToConvert = typeof<YamlNode>

    override _.CreateConverter (typeToConvert: Type, options:YamlSerializerOptions) =
        YamlNodeConverter()

type YamlNodeValueConverterFactory() =
    inherit YamlConverterFactory()

    override _.CanConvert (typeToConvert:Type) =
        match TypeCache.getKind typeToConvert with
        | TypeCache.TypeKind.FsUnion ->
            if typeToConvert.IsGenericType then
                let gen = typeToConvert.GetGenericTypeDefinition()
                gen = typedefof<YamlNodeValue<_>>
            else false
        | _ -> false

    override _.CreateConverter (typeToConvert: Type, options:YamlSerializerOptions) =
        match TypeCache.getKind typeToConvert with
        | TypeCache.TypeKind.FsUnion ->
            let converterType = typedefof<YamlNodeConverter<_>>
            converterType
                .MakeGenericType([| typeToConvert.GetGenericArguments().[0] |])
                .GetConstructor([| typeof<YamlSerializerOptions> |])
                .Invoke([| options |])
            :?> YamlConverter

        | _ -> failwith "Unknown type"
