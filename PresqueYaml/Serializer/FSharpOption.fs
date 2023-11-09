namespace PresqueYaml.Serializer
open PresqueYaml.Model
open System

type FSharpOptionConverter<'T>(options:YamlSerializerOptions) =
    inherit YamlConverter<'T option>()

    override _.Read(node:YamlNode, typeToConvert:Type) =
        match node with
        | YamlNode.None -> None
        | _ ->
            let data = YamlSerializer.Deserialize(node, typeof<'T>, options) :?> 'T
            Some data

type FSharpOptionConverterFactory() =
    inherit YamlConverterFactory()

    override _.CanConvert (typeToConvert:Type) =
        match TypeCache.getKind typeToConvert with
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
        | TypeCache.TypeKind.FsUnion ->
            let converterType = typedefof<FSharpOptionConverter<_>>
            
            converterType
                .MakeGenericType([| typeToConvert.GetGenericArguments().[0] |])
                .GetConstructor([| typeof<YamlSerializerOptions> |])
                .Invoke([| options |])
            :?> YamlConverter

        | _ -> failwith "Unknown type"
