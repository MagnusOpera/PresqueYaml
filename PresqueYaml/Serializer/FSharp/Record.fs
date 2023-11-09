namespace PresqueYaml.Serializer
open PresqueYaml.Model
open System
open Microsoft.FSharp.Reflection


type FSharpRecordConverter<'T when 'T : null>(options:YamlSerializerOptions) =
    inherit YamlConverter<'T>()

    let recordType = typeof<'T>
    let ctor = FSharpValue.PreComputeRecordConstructor(recordType, true)
    let fields = FSharpType.GetRecordFields(recordType, true)
    let fieldCount = fields.Length

    let isNullableUnion (ty: Type) =
        ty.GetCustomAttributes(typeof<CompilationRepresentationAttribute>, false)
        |> Array.exists (fun x ->
            let x = (x :?> CompilationRepresentationAttribute)
            x.Flags.HasFlag(CompilationRepresentationFlags.UseNullAsTrueValue))

    let isValueOptionType (ty: Type) =
        ty.IsGenericType && ty.GetGenericTypeDefinition() = typedefof<ValueOption<_>>

    let isClass ty =
        not (FSharpType.IsUnion(ty, true))
        && not (FSharpType.IsRecord(ty, true))
        && not (FSharpType.IsTuple(ty))

    let rec tryGetNullValue (ty: Type) : obj voption =
        if isNullableUnion ty then
            ValueSome null
        elif ty = typeof<unit> then
            ValueSome()
        elif isClass ty then
            ValueSome(if ty.IsValueType then Activator.CreateInstance(ty) else null)
        else
            ValueNone

    let defaultFields =
        let arr = Array.zeroCreate fieldCount
        
        fields
        |> Array.iteri (fun i field ->
            match tryGetNullValue field.PropertyType with
            | ValueSome v -> arr[i] <- v
            | ValueNone -> ())
        arr


    override _.Read(node:YamlNode, typeToConvert:Type) =
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
                        let data = YamlSerializer.Deserialize(node, propType, options)
                        fieldValues[index] <- data
                    | _ -> ()

        | _ -> ()

        ctor fieldValues :?> 'T
