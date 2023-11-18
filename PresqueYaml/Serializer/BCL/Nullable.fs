namespace MagnusOpera.PresqueYaml.Converters
open MagnusOpera.PresqueYaml
open System

type NullableConverter<'T when 'T: (new: unit -> 'T) and 'T: struct and 'T :> ValueType>() =
    inherit YamlConverter<Nullable<'T>>()

    override _.Read(node:YamlNode, serializer) =
        match node with
        | YamlNode.None -> Nullable<'T>()
        | _ ->
            let data = serializer.Deserialize("Nullable", node, typeof<'T>) :?> 'T
            Nullable<'T>(data)

