namespace PresqueYaml.Mapper
open PresqueYaml.Model
open System

type ConvertibleConverter<'T>(options:YamlSerializerOptions) =
    inherit YamlConverter<'T>()

    override _.Read(node:YamlNode, typeToConvert:Type) =
        match node with
        | YamlNode.Scalar data -> Convert.ChangeType(data, typeToConvert) :?> 'T
        | _ -> failwith $"Failed to convert to {typeToConvert.Name}"


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

    override _.CreateConverter (typeToConvert:Type, options:YamlSerializerOptions) =
        if typeToConvert = typeof<Int16> then ConvertibleConverter<Int16>(options)
        elif typeToConvert = typeof<UInt16> then ConvertibleConverter<UInt16>(options)
        elif typeToConvert = typeof<Int32> then ConvertibleConverter<Int32>(options)
        elif typeToConvert = typeof<UInt32> then ConvertibleConverter<UInt32>(options)
        elif typeToConvert = typeof<Int64> then ConvertibleConverter<Int64>(options)
        elif typeToConvert = typeof<UInt64> then ConvertibleConverter<UInt64>(options)
        elif typeToConvert = typeof<Char> then ConvertibleConverter<Char>(options)
        elif typeToConvert = typeof<Byte> then ConvertibleConverter<Byte>(options)
        elif typeToConvert = typeof<String> then ConvertibleConverter<String>(options)
        else failwith "Unknown type"


