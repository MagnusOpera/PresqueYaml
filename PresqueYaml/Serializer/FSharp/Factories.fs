namespace MagnusOpera.PresqueYaml.Converters
open System
open MagnusOpera.PresqueYaml

type FSharpCollectionsConverterFactory() =
    inherit YamlConverterFactory()

    override _.CanConvert (typeToConvert:Type) =
        match TypeHelpers.getKind typeToConvert with
        | TypeHelpers.TypeKind.FsList -> true
        | TypeHelpers.TypeKind.FsSet -> true
        | TypeHelpers.TypeKind.FsMap -> true
        | _ -> false

    override _.CreateConverter (typeToConvert: Type) =
        let converterType, idx =
            match TypeHelpers.getKind typeToConvert with
            | TypeHelpers.TypeKind.FsList -> typedefof<FSharpListConverter<_>>, 0
            | TypeHelpers.TypeKind.FsSet -> typedefof<FSharpSetConverter<_>>, 0
            | TypeHelpers.TypeKind.FsMap -> typedefof<FSharpMapConverter<_>>, 1
            | _ -> YamlSerializerException.Raise "Unknown type"

        converterType
            .MakeGenericType([| typeToConvert.GetGenericArguments().[idx] |])
            .GetConstructor([| |])
            .Invoke([| |])
        :?> YamlConverter


type FSharpUnionConverterFactory() =
    inherit YamlConverterFactory()

    override _.CanConvert (typeToConvert:Type) =
        match TypeHelpers.getKind typeToConvert with
        | TypeHelpers.TypeKind.FsUnion ->
            if typeToConvert.IsGenericType then
                let gen = typeToConvert.GetGenericTypeDefinition()
                if gen = typedefof<option<_>> then true
                elif gen = typedefof<voption<_>> then true
                elif gen = typedefof<YamlNodeValue<_>> then true
                else false
            else false
        | _ -> false

    override _.CreateConverter (typeToConvert: Type) =
        match TypeHelpers.getKind typeToConvert with
        | TypeHelpers.TypeKind.FsUnion ->
            let converterType = typedefof<FSharpOptionConverter<_>>

            converterType
                .MakeGenericType([| typeToConvert.GetGenericArguments().[0] |])
                .GetConstructor([| |])
                .Invoke([| |])
            :?> YamlConverter

        | _ -> YamlSerializerException.Raise "Unknown type"


type FSharpRecordConverterFactory() =
    inherit YamlConverterFactory()

    override _.CanConvert (typeToConvert:Type) =
        match TypeHelpers.getKind typeToConvert with
        | TypeHelpers.TypeKind.FsRecord -> true
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
        match TypeHelpers.getKind typeToConvert with
        | TypeHelpers.TypeKind.FsUnit -> true
        | _ -> false

    override _.CreateConverter (typeToConvert: Type) =
        FSharpUnitConverter()
