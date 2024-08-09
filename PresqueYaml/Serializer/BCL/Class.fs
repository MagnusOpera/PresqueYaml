namespace MagnusOpera.PresqueYaml.Converters
open MagnusOpera.PresqueYaml

[<Sealed>]
type ClassConverter<'T when 'T : null>() =
    inherit YamlConverter<'T>()

    let classType = typeof<'T>
    let ctor = classType.GetConstructors() |> Seq.exactlyOne
    let parameters = ctor.GetParameters()

    override _.Read(node, options, serializer) =
        let parameterRequired =
            parameters
            |> Array.map (TypeHelpers.getParameterRequired options.NoneIsEmpty)

        let parameterValues =
            parameters
            |> Array.map (fun info -> serializer.Default(info.ParameterType))

        let parameterIndices =
            parameters
            |> Seq.mapi (fun idx pi  ->
                (pi.Name |> nonNull).ToLowerInvariant(), idx)
            |> Map

        match node with
        | YamlNode.None -> null
        | YamlNode.Mapping mapping ->
            for KeyValue(name, node) in mapping do
                match parameterIndices |> Map.tryFind (name.ToLowerInvariant()) with
                | Some index ->
                    let propType = parameters[index].ParameterType
                    let name = parameters[index].Name |> nonNull
                    let data = serializer.Deserialize(name, node, propType)
                    parameterValues[index] <- data
                    parameterRequired[index] <- false
                | _ -> ()

            let requiredIndex = parameterRequired |> Array.tryFindIndex id
            match requiredIndex with
            | Some idx -> YamlSerializerException.Raise($"parameter {parameters[idx].Name} must be provided")
            | _ -> ctor.Invoke(parameterValues) :?> 'T

        | _ -> YamlSerializerException.Raise("can't convert sequence or mapping to record")
