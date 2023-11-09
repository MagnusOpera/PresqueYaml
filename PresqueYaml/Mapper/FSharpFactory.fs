namespace PresqueYaml.Mapper
open System

type FSharpConverterFactory() =
    inherit YamlConverterFactory()

    override _.CanConvert (typeToConvert:Type) =
        match TypeCache.getKind typeToConvert with
        | TypeCache.TypeKind.FsList -> true
        | TypeCache.TypeKind.FsSet -> true
        | TypeCache.TypeKind.FsMap -> true
        | TypeCache.TypeKind.FsTuple -> false
        | TypeCache.TypeKind.FsRecord -> true
        | TypeCache.TypeKind.FsUnion ->
            if typeToConvert.IsGenericType then
                let gen = typeToConvert.GetGenericTypeDefinition()
                if gen = typedefof<option<_>> then true
                elif gen = typedefof<voption<_>> then true
                else false
            else false
        | _ -> false

    override _.CreateConverter (typeToConvert: Type, options:YamlSerializerOptions) =
        match TypeCache.getKind typeToConvert with
        | TypeCache.TypeKind.FsRecord ->
            FSharpRecordConverter(options)
        | TypeCache.TypeKind.FsUnion ->
            if typeToConvert.IsGenericType then
                let gen = typeToConvert.GetGenericTypeDefinition()
                if gen = typedefof<option<_>> then FSharpOptionConverter(options)
                elif gen = typedefof<voption<_>> then FSharpOptionConverter(options)
                else failwith "Unknown type"
            else failwith "Unknown type"
        | _ ->
            let converterType, idx =
                match TypeCache.getKind typeToConvert with
                | TypeCache.TypeKind.FsList -> typedefof<FSharpListConverter<_>>, 0
                | TypeCache.TypeKind.FsSet -> typedefof<FSharpSetConverter<_>>, 0
                | TypeCache.TypeKind.FsMap -> typedefof<FSharpMapConverter<_>>, 1
                | _ -> failwith "Unknown type"

            converterType
                .MakeGenericType([| typeToConvert.GetGenericArguments().[idx] |])
                .GetConstructor([| typeof<YamlSerializerOptions> |])
                .Invoke([| options |])
            :?> YamlConverter
