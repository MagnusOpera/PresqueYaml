namespace MagnusOpera.PresqueYaml.Converters
open System
open MagnusOpera.PresqueYaml
open TypeHelpers

[<Sealed>]
type FSharpCollectionsConverterFactory() =
    inherit YamlConverterFactory()

    override _.CanConvert (typeToConvert:Type) =
        match getKind typeToConvert with
        | TypeKind.FsList
        | TypeKind.FsSet
        | TypeKind.FsMap -> true
        | _ -> false

    override _.CreateConverter (typeToConvert: Type, options:YamlSerializerOptions) =
        let converterType, idx =
            match getKind typeToConvert with
            | TypeKind.FsList -> typedefof<FSharpListConverter<_>>, 0
            | TypeKind.FsSet -> typedefof<FSharpSetConverter<_>>, 0
            | TypeKind.FsMap -> typedefof<FSharpMapConverter<_>>, 1
            | _ -> YamlSerializerException.Raise("unknown type")
        let ctor =
            converterType
                .MakeGenericType([| typeToConvert.GetGenericArguments().[idx] |])
                .GetConstructor([| |]) |> nonNull
        ctor.Invoke([| |]) :?> YamlConverter


[<Sealed>]
type FSharpUnionConverterFactory() =
    inherit YamlConverterFactory()

    override _.CanConvert (typeToConvert:Type) =
        match getKind typeToConvert with
        | TypeKind.FsOption
        | TypeKind.FsVOption -> true
        | _ -> false

    override _.CreateConverter (typeToConvert: Type, options:YamlSerializerOptions) =
        let converterType = typedefof<FSharpOptionConverter<_>>
        let ctor =
            converterType
                .MakeGenericType([| typeToConvert.GetGenericArguments().[0] |])
                .GetConstructor([| |]) |> nonNull
        ctor.Invoke([| |]) :?> YamlConverter


[<Sealed>]
type FSharpRecordConverterFactory() =
    inherit YamlConverterFactory()

    override _.CanConvert (typeToConvert:Type) =
        match getKind typeToConvert with
        | TypeKind.FsRecord -> true
        | _ -> false

    override _.CreateConverter (typeToConvert: Type, options:YamlSerializerOptions) =
        let converterType = typedefof<FSharpRecordConverter<_>>
        let ctor =
            converterType
                .MakeGenericType([| typeToConvert |])
                .GetConstructor([| |]) |> nonNull
        ctor.Invoke([| |]) :?> YamlConverter


[<Sealed>]
type FSharpUnitConverterFactory() =
    inherit YamlConverterFactory()

    override _.CanConvert (typeToConvert:Type) =
        match getKind typeToConvert with
        | TypeKind.FsUnit -> true
        | _ -> false

    override _.CreateConverter (typeToConvert: Type, options:YamlSerializerOptions) =
        FSharpUnitConverter()
