namespace PresqueYaml.Mapper
open PresqueYaml.Model
open System

type ArrayConverter<'T when 'T: comparison>(options:YamlSerializerOptions) =
    inherit YamlConverter<'T[]>()

    override _.Read(node:YamlNode, typeToConvert:Type) =
        match node with
        | YamlNode.Sequence sequence ->
            sequence
            |> Seq.map (fun node -> YamlSerializer.Deserialize(node, typeof<'T>, options) :?> 'T)
            |> Array.ofSeq
        | _ -> failwith $"Failed to convert to {typeToConvert.Name}"

type ArrayConverterFactory() =
    inherit YamlConverterFactory()

    override _.CanConvert (typeToConvert:Type) =
        TypeCache.isArray typeToConvert

    override _.CreateConverter (typeToConvert:Type, options:YamlSerializerOptions) =
        let converterType = typedefof<ArrayConverter<_>>

        converterType
            .MakeGenericType([| typeToConvert.GetElementType() |])
            .GetConstructor([| typeof<YamlSerializerOptions> |])
            .Invoke([| options |])
        :?> YamlConverter
