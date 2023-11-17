namespace MagnusOpera.PresqueYaml.Converters
open System
open MagnusOpera.PresqueYaml

type FSharpCollectionsConverterFactory() =
    inherit YamlConverterFactory()

    override _.CanConvert (typeToConvert:Type) =
        match TypeCache.getKind typeToConvert with
        | TypeCache.TypeKind.FsList -> true
        | TypeCache.TypeKind.FsSet -> true
        | TypeCache.TypeKind.FsMap -> true
        | _ -> false

    override _.CreateConverter (typeToConvert: Type) =
        let converterType, idx =
            match TypeCache.getKind typeToConvert with
            | TypeCache.TypeKind.FsList -> typedefof<FSharpListConverter<_>>, 0
            | TypeCache.TypeKind.FsSet -> typedefof<FSharpSetConverter<_>>, 0
            | TypeCache.TypeKind.FsMap -> typedefof<FSharpMapConverter<_>>, 1
            | _ -> failwith "Unknown type"

        converterType
            .MakeGenericType([| typeToConvert.GetGenericArguments().[idx] |])
            .GetConstructor([| |])
            .Invoke([| |])
        :?> YamlConverter


type FSharpUnionConverterFactory() =
    inherit YamlConverterFactory()

    override _.CanConvert (typeToConvert:Type) =
        match TypeCache.getKind typeToConvert with
        | TypeCache.TypeKind.FsUnion ->
            if typeToConvert.IsGenericType then
                let gen = typeToConvert.GetGenericTypeDefinition()
                if gen = typedefof<option<_>> then true
                elif gen = typedefof<voption<_>> then true
                elif gen = typedefof<YamlNodeValue<_>> then true
                else false
            else false
        | _ -> false

    override _.CreateConverter (typeToConvert: Type) =
        match TypeCache.getKind typeToConvert with
        | TypeCache.TypeKind.FsUnion ->
            let converterType = typedefof<FSharpOptionConverter<_>>

            converterType
                .MakeGenericType([| typeToConvert.GetGenericArguments().[0] |])
                .GetConstructor([| |])
                .Invoke([| |])
            :?> YamlConverter

        | _ -> failwith "Unknown type"


type FSharpRecordConverterFactory() =
    inherit YamlConverterFactory()

    override _.CanConvert (typeToConvert:Type) =
        match TypeCache.getKind typeToConvert with
        | TypeCache.TypeKind.FsRecord -> true
        | _ -> false

    override _.CreateConverter (typeToConvert: Type) =
        let converterType = typedefof<FSharpRecordConverter<_>>
        converterType
            .MakeGenericType([| typeToConvert |])
            .GetConstructor([| |])
            .Invoke([| |])
        :?> YamlConverter


type FSharpUnitConverterFactory() =
    inherit YamlConverterFactory()

    override _.CanConvert (typeToConvert:Type) =
        match TypeCache.getKind typeToConvert with
        | TypeCache.TypeKind.FsUnit -> true
        | _ -> false

    override _.CreateConverter (typeToConvert: Type) =
        FSharpUnitConverter()
