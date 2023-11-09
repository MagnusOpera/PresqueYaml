namespace PresqueYaml.Serializer
open PresqueYaml
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
