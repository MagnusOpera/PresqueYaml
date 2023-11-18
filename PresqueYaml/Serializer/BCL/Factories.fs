namespace MagnusOpera.PresqueYaml.Converters
open System
open MagnusOpera.PresqueYaml

type CollectionConverterFactory() =
    inherit YamlConverterFactory()

    override _.CanConvert (typeToConvert:Type) =
        match TypeHelpers.getKind typeToConvert with
        | TypeHelpers.TypeKind.List
        | TypeHelpers.TypeKind.Dictionary -> true
        | _ -> false

    override _.CreateConverter (typeToConvert:Type) =

        let converterType, idx =
            match TypeHelpers.getKind typeToConvert with
            | TypeHelpers.TypeKind.List -> typedefof<ListConverter<_>>, 0
            | TypeHelpers.TypeKind.Dictionary -> typedefof<DictionaryConverter<_>>, 1
            | _ -> failwith "Unknown type"

        converterType
            .MakeGenericType([| typeToConvert.GetGenericArguments().[idx] |])
            .GetConstructor([| |])
            .Invoke([| |])
        :?> YamlConverter


type ArrayConverterFactory() =
    inherit YamlConverterFactory()

    override _.CanConvert (typeToConvert:Type) =
        match TypeHelpers.getKind typeToConvert with
        | TypeHelpers.TypeKind.Array -> true
        | _ -> false

    override _.CreateConverter (typeToConvert:Type) =
        let converterType = typedefof<ArrayConverter<_>>

        converterType
            .MakeGenericType([| typeToConvert.GetElementType() |])
            .GetConstructor([| |])
            .Invoke([| |])
        :?> YamlConverter


type ConvertibleConverterFactory() =
    inherit YamlConverterFactory()

    override _.CanConvert (typeToConvert:Type) =
        typeToConvert = typeof<Int16>
        || typeToConvert = typeof<UInt16>
        || typeToConvert = typeof<Int32>
        || typeToConvert = typeof<UInt32>
        || typeToConvert = typeof<Int64>
        || typeToConvert = typeof<UInt64>
        || typeToConvert = typeof<Char>
        || typeToConvert = typeof<Byte>
        || typeToConvert = typeof<String>

    override _.CreateConverter (typeToConvert:Type) =
        if typeToConvert = typeof<Int16> then ConvertibleConverter<Int16>()
        elif typeToConvert = typeof<UInt16> then ConvertibleConverter<UInt16>()
        elif typeToConvert = typeof<Int32> then ConvertibleConverter<Int32>()
        elif typeToConvert = typeof<UInt32> then ConvertibleConverter<UInt32>()
        elif typeToConvert = typeof<Int64> then ConvertibleConverter<Int64>()
        elif typeToConvert = typeof<UInt64> then ConvertibleConverter<UInt64>()
        elif typeToConvert = typeof<Char> then ConvertibleConverter<Char>()
        elif typeToConvert = typeof<Byte> then ConvertibleConverter<Byte>()
        elif typeToConvert = typeof<String> then ConvertibleConverter<String>()
        else failwith "Unknown type"


type NullableConverterFactory() =
    inherit YamlConverterFactory()

    override _.CanConvert (typeToConvert:Type) =
        match TypeHelpers.getKind typeToConvert with
        | TypeHelpers.TypeKind.Nullable -> true
        | _ -> false

    override _.CreateConverter (typeToConvert:Type) =

        let converterType = typedefof<NullableConverter<_>>

        converterType
            .MakeGenericType([| typeToConvert.GetGenericArguments().[0] |])
            .GetConstructor([| |])
            .Invoke([| |])
        :?> YamlConverter


type ClassConverterFactory() =
    inherit YamlConverterFactory()

    override _.CanConvert (typeToConvert:Type) =
        match TypeHelpers.getKind typeToConvert with
        | TypeHelpers.TypeKind.Other -> true
        | _ -> false

    override _.CreateConverter (typeToConvert: Type) =
        let converterType = typedefof<ClassConverter<_>>
        converterType
            .MakeGenericType([| typeToConvert |])
            .GetConstructor([| |])
            .Invoke([| |])
        :?> YamlConverter
