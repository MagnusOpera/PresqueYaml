namespace PresqueYaml.Mapper
open System

type FSharpConverterFactory() =
    inherit YamlConverterFactory()

    override _.CanConvert (typeToConvert:Type) =
        match TypeCache.getKind typeToConvert with
        | TypeCache.TypeKind.List -> true
        | TypeCache.TypeKind.Set -> true
        | TypeCache.TypeKind.Map -> true
        | TypeCache.TypeKind.Tuple -> false
        | TypeCache.TypeKind.Record -> true
        | TypeCache.TypeKind.Union ->
            if typeToConvert.IsGenericType then
                let gen = typeToConvert.GetGenericTypeDefinition()
                if gen = typedefof<option<_>> then true
                elif gen = typedefof<voption<_>> then true
                else false
            else false
        | _ -> false

    override _.CreateConverter (typeToConvert: Type, options:YamlSerializerOptions) =
        match TypeCache.getKind typeToConvert with
        | TypeCache.TypeKind.Record ->
            FSharpRecordConverter(options)
        | TypeCache.TypeKind.Union ->
            if typeToConvert.IsGenericType then
                let gen = typeToConvert.GetGenericTypeDefinition()
                if gen = typedefof<option<_>> then FSharpOptionConverter(options)
                elif gen = typedefof<voption<_>> then FSharpOptionConverter(options)
                else failwith "Unknown type"
            else failwith "Unknown type"
        | _ ->
            let converterType =
                match TypeCache.getKind typeToConvert with
                | TypeCache.TypeKind.List -> typedefof<FSharpListConverter<_>>
                | TypeCache.TypeKind.Set -> typedefof<FSharpSetConverter<_>>
                | TypeCache.TypeKind.Map -> typedefof<FSharpMapConverter<_>>
                | _ -> failwith "Unknown type"

            converterType
                .MakeGenericType([| typeToConvert.GetGenericArguments().[0] |])
                .GetConstructor([| typeof<YamlSerializerOptions> |])
                .Invoke([| options |])
            :?> YamlConverter

