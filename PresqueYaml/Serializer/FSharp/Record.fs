namespace MagnusOpera.PresqueYaml.Converters
open MagnusOpera.PresqueYaml
open Microsoft.FSharp.Reflection

type FSharpRecordConverter<'T when 'T : null>() =
    inherit YamlConverter<'T>()

    let recordType = typeof<'T>
    let ctor = FSharpValue.PreComputeRecordConstructor(recordType, true)
    let fields = FSharpType.GetRecordFields(recordType, true)

    let parameterRequired =
        fields
        |> Array.map TypeHelpers.getPropertyRequired

    override _.Read(node:YamlNode, serializer) =
        let fieldRequired = Array.copy parameterRequired

        let fieldValues =
            fields
            |> Array.map (fun info -> serializer.Default(info.PropertyType))

        let fieldIndices =
            fields
            |> Seq.mapi (fun idx pi  -> pi.Name.ToLowerInvariant(), idx)
            |> Map

        match node with
        | YamlNode.Mapping mapping ->
            for KeyValue(name, node) in mapping do
                match fieldIndices |> Map.tryFind (name.ToLowerInvariant()) with
                | Some index ->
                    let propType = fields[index].PropertyType
                    let data = serializer.Deserialize(fields[index].Name, node, propType)
                    fieldValues[index] <- data
                    fieldRequired[index] <- false
                | _ -> ()

            let requiredIndex = fieldRequired |> Array.tryFindIndex id
            match requiredIndex with
            | Some idx -> YamlSerializerException.Raise $"Parameter {fields[idx].Name} must be provided"
            | _ -> ctor fieldValues :?> 'T
        | _ -> YamlSerializerException.Raise "Can't convert None, Sequence or Mapping to record"
