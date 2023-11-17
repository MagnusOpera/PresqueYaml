namespace MagnusOpera.PresqueYaml.Converters
open System.Reflection
open MagnusOpera.PresqueYaml
open System

type ClassConverter<'T when 'T : null>() =
    inherit YamlConverter<'T>()

    let classType = typeof<'T>
    let ctor = classType.GetConstructors() |> Seq.exactlyOne
    let parameters = ctor.GetParameters()

    override _.Read(node:YamlNode, typeToConvert:Type, serializer) =
        let nrtContext = NullabilityInfoContext()

        let requiredParameters =
            parameters
            |> Array.map (fun parameter ->
                let nrtInfo = nrtContext.Create(parameter)
                nrtInfo.ReadState = NullabilityState.NotNull)

        let defaultParameters =
            parameters
            |> Array.map (fun parameter -> serializer.Default(parameter.ParameterType))

        let parameterIndices =
            parameters
            |> Seq.mapi (fun idx pi  -> pi.Name.ToLowerInvariant(), idx)
            |> Map

        let parameterValues = Array.copy defaultParameters

        match node with
        | YamlNode.None -> null
        | YamlNode.Mapping mapping ->
            for KeyValue(name, node) in mapping do
                match node with
                | YamlNode.None -> ()
                | _ ->
                    match parameterIndices |> Map.tryFind (name.ToLowerInvariant()) with
                    | Some index ->
                        let propType = parameters[index].ParameterType
                        let data = serializer.Deserialize(parameters[index].Name, node, propType)
                        parameterValues[index] <- data
                        requiredParameters[index] <- false
                    | _ -> ()

            let requiredIndex = requiredParameters |> Array.tryFindIndex id
            match requiredIndex with
            | Some idx -> failwith $"Parameter {parameters[idx].Name} must be provided"
            | _ -> ctor.Invoke(parameterValues) :?> 'T

        | _ -> failwith "Can't convert sequence or mapping to record"
