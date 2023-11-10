namespace PresqueYaml.Serializer
open PresqueYaml
open System

type ConvertibleConverter<'T>(options:YamlSerializerOptions) =
    inherit YamlConverter<'T>()

    override _.Read(node:YamlNode, typeToConvert:Type) =
        match node with
        | YamlNode.None ->
            if options.NoneIsDefault then Unchecked.defaultof<'T>
            else failwith "Failed to convert None to convertible"
        | YamlNode.Scalar data -> Convert.ChangeType(data, typeToConvert) :?> 'T
        | _ -> failwith $"Failed to convert to {typeToConvert.Name}"
