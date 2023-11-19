namespace MagnusOpera.PresqueYaml.Converters
open MagnusOpera.PresqueYaml
open System


[<Sealed>]
type ConvertibleConverter<'T>() =
    inherit YamlConverter<'T>()

    override _.Read(node, options, serializer) =
        match node with
        | YamlNode.None ->
            if options.NoneIsEmpty then Unchecked.defaultof<'T>
            else YamlSerializerException.Raise "failed to convert None to convertible"
        | YamlNode.Scalar data -> Convert.ChangeType(data, typeof<'T>) :?> 'T
        | _ -> YamlSerializerException.Raise $"failed to convert to {typeof<'T>.Name}"
