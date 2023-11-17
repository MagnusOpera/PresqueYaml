namespace MagnusOpera.PresqueYaml.Converters
open System.Reflection
open MagnusOpera.PresqueYaml
open System
open Microsoft.FSharp.Reflection


type FSharpRecordConverter<'T when 'T : null>() =
    inherit YamlConverter<'T>()

    let recordType = typeof<'T>
    let ctor = FSharpValue.PreComputeRecordConstructor(recordType, true)
    let fields = FSharpType.GetRecordFields(recordType, true)

    override _.Read(node:YamlNode, typeToConvert:Type, serializer) =
        let nrtContext = NullabilityInfoContext()

        let requiredFields =
            fields
            |> Array.map (fun parameter ->
                let nrtInfo = nrtContext.Create(parameter)
                nrtInfo.ReadState = NullabilityState.NotNull)

        let defaultFields =
            fields
            |> Array.map (fun field -> serializer.Default(field.PropertyType))

        let fieldIndices =
            fields
            |> Seq.mapi (fun idx pi  -> pi.Name.ToLowerInvariant(), idx)
            |> Map

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
                        let data = serializer.Deserialize(fields[index].Name, node, propType)
                        fieldValues[index] <- data
                        requiredFields[index] <- false
                    | _ -> ()

            let requiredIndex = requiredFields |> Array.tryFindIndex id
            match requiredIndex with
            | Some idx -> failwith $"Parameter {fields[idx].Name} must be provided"
            | _ -> ctor fieldValues :?> 'T
        | _ -> failwith "Can't convert None, Sequence or Mapping to record"
