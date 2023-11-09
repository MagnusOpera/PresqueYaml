namespace PresqueYaml.Serializer
open PresqueYaml.Model
open System

type NullableConverter<'T when 'T: (new: unit -> 'T) and 'T: struct and 'T :> ValueType>(options:YamlSerializerOptions) =
    inherit YamlConverter<Nullable<'T>>()

    override _.Read(node:YamlNode, typeToConvert:Type) =
        match node with
        | YamlNode.None -> Nullable<'T>()
        | _ ->
            let data = YamlSerializer.Deserialize(node, typeof<'T>, options) :?> 'T
            Nullable<'T>(data)


type NullableConverterFactory() =
    inherit YamlConverterFactory()

    override _.CanConvert (typeToConvert:Type) =
        TypeCache.isNullable typeToConvert

    override _.CreateConverter (typeToConvert:Type, options:YamlSerializerOptions) =

        let converterType = typedefof<NullableConverter<_>>

        converterType
            .MakeGenericType([| typeToConvert.GetGenericArguments().[0] |])
            .GetConstructor([| typeof<YamlSerializerOptions> |])
            .Invoke([| options |])
        :?> YamlConverter
