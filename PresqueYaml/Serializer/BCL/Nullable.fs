namespace MagnusOpera.PresqueYaml.Converters
open MagnusOpera.PresqueYaml
open System

type NullableConverter<'T when 'T: (new: unit -> 'T) and 'T: struct and 'T :> ValueType>(options:YamlSerializerOptions) =
    inherit YamlConverter<Nullable<'T>>()

    override _.Read(node:YamlNode, typeToConvert:Type) =
        match node with
        | YamlNode.None -> Nullable<'T>()
        | _ ->
            let data = YamlSerializer.Deserialize(node, typeof<'T>, options) :?> 'T
            Nullable<'T>(data)

