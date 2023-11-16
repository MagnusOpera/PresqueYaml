namespace MagnusOpera.PresqueYaml.Converters
open MagnusOpera.PresqueYaml
open System

type ClassConverter<'T when 'T : null>(options:YamlSerializerOptions) =
    inherit YamlConverter<'T>()

    let classType = typeof<'T>
    let ctor = classType.GetConstructors() |> Seq.exactlyOne
    let parameters = ctor.GetParameters()

    let defaultParameters =
        parameters
        |> Array.map (fun parameter -> YamlSerializer.Default(parameter.ParameterType, options))

    let parameterIndices =
        parameters
        |> Seq.mapi (fun idx pi  -> pi.Name.ToLowerInvariant(), idx)
        |> Map

    override _.Read(node:YamlNode, typeToConvert:Type) =
        let parameterValues = Array.copy defaultParameters

        match node with
        | YamlNode.None -> null
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
            ctor.Invoke(parameterValues) :?> 'T
        | _ -> failwith "Can't convert sequence or mapping to record"
