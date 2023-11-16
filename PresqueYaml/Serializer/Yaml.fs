namespace MagnusOpera.PresqueYaml.Converters
open MagnusOpera.PresqueYaml
open System

type YamlNodeConverter() =
    inherit YamlConverter<YamlNode>()

    override _.CanConvert(typeToConvert:Type) =
        typeToConvert = typeof<YamlNode>

    override _.Read(node:YamlNode, typeToConvert:Type) =
        node


type YamlOption<'T> =
    | Undefined
    | None
    | Some of value:'T

type YamlOptionConverter<'T>() =
    inherit YamlConverter<YamlOption<'T>>()

    override _.Read(node:YamlNode, typeToConvert:Type) =
        Undefined


type YamlOptionConverterFactory() =
    inherit YamlConverterFactory()

    override _.CanConvert (typeToConvert:Type) =
        match TypeCache.getKind typeToConvert with
        | TypeCache.TypeKind.FsUnion ->
            if typeToConvert.IsGenericType then
                let gen = typeToConvert.GetGenericTypeDefinition()
                if gen = typedefof<YamlOption<_>> then true
                else false
            else false
        | _ -> false

    override _.CreateConverter (typeToConvert: Type, options:YamlSerializerOptions) =
        match TypeCache.getKind typeToConvert with
        | TypeCache.TypeKind.FsUnion ->
            let converterType = typedefof<YamlOptionConverter<_>>

            converterType
                .MakeGenericType([| typeToConvert.GetGenericArguments().[0] |])
                .GetConstructor([| typeof<YamlSerializerOptions> |])
                .Invoke([| options |])
            :?> YamlConverter

        | _ -> failwith "Unknown type"
