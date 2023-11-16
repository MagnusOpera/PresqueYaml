namespace MagnusOpera.PresqueYaml.Converters
open MagnusOpera.PresqueYaml
open System
open Microsoft.FSharp.Reflection


type FSharpRecordConverter<'T when 'T : null>(options:YamlSerializerOptions) =
    inherit YamlConverter<'T>()

    let recordType = typeof<'T>
    let ctor = FSharpValue.PreComputeRecordConstructor(recordType, true)
    let fields = FSharpType.GetRecordFields(recordType, true)

    let defaultFields =
        fields
        |> Array.map (fun field ->
            YamlSerializer.Deserialize(YamlNode.None, field.PropertyType, options))

    let fieldIndices =
        fields
        |> Seq.mapi (fun idx pi  -> pi.Name.ToLowerInvariant(), idx)
        |> Map

    override _.Read(node:YamlNode, typeToConvert:Type) =
        let fieldValues = Array.copy defaultFields

        match node with
        | YamlNode.Mapping mapping ->
            for (KeyValue(name, node)) in mapping do
                match node with
                | YamlNode.None -> ()
                | _ ->
                    match fieldIndices |> Map.tryFind (name.ToLowerInvariant()) with
                    | Some index ->
                        let propType = fields[index].PropertyType
                        let data = YamlSerializer.Deserialize(node, propType, options)
                        fieldValues[index] <- data
                    | _ -> ()
            ctor fieldValues :?> 'T
        | _ -> failwith "Can't convert None, Sequence or Mapping to record"
