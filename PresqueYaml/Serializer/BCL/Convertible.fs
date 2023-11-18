namespace MagnusOpera.PresqueYaml.Converters
open MagnusOpera.PresqueYaml
open System

type ConvertibleConverter<'T>() =
    inherit YamlConverter<'T>()

    override _.Default options =
        if options.NoneIsEmpty then null
        else failwith "Failed to convert None to convertible"

    override _.Read(node:YamlNode, serializer) =
        match node with
        | YamlNode.None ->
            if serializer.Options.NoneIsEmpty then Unchecked.defaultof<'T>
            else failwith "Failed to convert None to convertible"
        | YamlNode.Scalar data -> Convert.ChangeType(data, typeof<'T>) :?> 'T
        | _ -> failwith $"Failed to convert to {typeof<'T>.Name}"
