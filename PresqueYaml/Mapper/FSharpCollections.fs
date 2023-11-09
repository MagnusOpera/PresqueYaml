namespace PresqueYaml.Mapper
open PresqueYaml.Model
open System

type FSharpListConverter<'T>(options:YamlSerializerOptions) =
    inherit YamlConverter<list<'T>>()

    override _.Read(node:YamlNode, typeToConvert:Type) =
        match node with
        | YamlNode.Sequence sequence ->
            sequence
            |> List.map (fun node -> YamlSerializer.Deserialize(node, typeof<'T>, options) :?> 'T)
        | _ -> failwith $"Failed to convert to {typeToConvert.Name}"

type FSharpSetConverter<'T when 'T: comparison>(options:YamlSerializerOptions) =
    inherit YamlConverter<Set<'T>>()

    override _.Read(node:YamlNode, typeToConvert:Type) =
        match node with
        | YamlNode.Sequence sequence ->
            sequence
            |> Seq.map (fun node -> YamlSerializer.Deserialize(node, typeof<'T>, options) :?> 'T)
            |> Set
        | _ -> failwith $"Failed to convert to {typeToConvert.Name}"

type FSharpMapConverter<'T>(options:YamlSerializerOptions) =
    inherit YamlConverter<Map<string, 'T>>()

    override _.Read(node:YamlNode, typeToConvert:Type) =
        match node with
        | YamlNode.Mapping mapping ->
            mapping
            |> Map.map (fun key node -> YamlSerializer.Deserialize(node, typeof<'T>, options) :?> 'T)
        | _ -> failwith $"Failed to convert to {typeToConvert.Name}"

type FSharpCollectionsConverterFactory() =
    inherit YamlConverterFactory()

    override _.CanConvert (typeToConvert:Type) =
        match TypeCache.getKind typeToConvert with
        | TypeCache.TypeKind.FsList -> true
        | TypeCache.TypeKind.FsSet -> true
        | TypeCache.TypeKind.FsMap -> true
        | _ -> false

    override _.CreateConverter (typeToConvert: Type, options:YamlSerializerOptions) =
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
