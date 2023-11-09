namespace PresqueYaml.Serializer
open PresqueYaml
open System

type ClassConverter<'T when 'T : null>(options:YamlSerializerOptions) =
    inherit YamlConverter<'T>()

    let classType = typeof<'T>
    let ctor = classType.GetConstructors() |> Seq.exactlyOne
    let parameters = ctor.GetParameters()
    let parameterCount = parameters.Length

    let defaultParameters =
        let arr = Array.zeroCreate parameterCount
        
        parameters
        |> Array.iteri (fun i parameter ->
            match Helpers.tryGetNullValue parameter.ParameterType with
            | ValueSome v -> arr[i] <- v
            | ValueNone -> ())
        arr

    let parameterIndices =
        parameters
        |> Seq.mapi (fun idx pi  -> pi.Name.ToLowerInvariant(), idx)
        |> Map

    override _.Read(node:YamlNode, typeToConvert:Type) =
        let parameterValues = Array.copy defaultParameters

        match node with
        | YamlNode.Mapping mapping ->
            for (KeyValue(name, node)) in mapping do
                match node with
                | YamlNode.None -> ()
                | _ ->
                    match parameterIndices |> Map.tryFind (name.ToLowerInvariant()) with
                    | Some index -> 
                        let propType = parameters[index].ParameterType
                        let data = YamlSerializer.Deserialize(node, propType, options)
                        parameterValues[index] <- data
                    | _ -> ()

        | _ -> ()

        ctor.Invoke(parameterValues) :?> 'T
