namespace MagnusOpera.PresqueYaml
open System
open MagnusOpera.PresqueYaml
open System.Runtime.InteropServices


[<Sealed>]
type private YamlSerializerContext(options:YamlSerializerOptions) =
    let getConverter (returnType:Type) =
        let factory =
            options.Converters
            |> List.tryFind (fun converter -> converter.CanConvert returnType)
        match factory with
        | Some factory ->
            let converter = factory.CreateConverter(returnType, options)
            converter
        | _ ->
            YamlParserException.Raise $"type {returnType.Name} has no registered converter"

    member val Contexts = System.Collections.Generic.Stack<string>()

    interface IYamlSerializer with
        member this.Default(returnType: Type): obj =
            let converter = getConverter returnType
            let defaultMethodInfo = converter.GetType() |> TypeHelpers.getDefault
            defaultMethodInfo.Invoke(converter, [| options |])

        member this.Deserialize(context: string, node: YamlNode, returnType: Type): obj =
            this.Contexts.Push(context)
            let serializer = this :> IYamlSerializer
            let converter = getConverter returnType
            let readMethodInfo = converter.GetType() |> TypeHelpers.getRead
            let res = readMethodInfo.Invoke(converter, [| node; options; serializer |])
            this.Contexts.Pop() |> ignore
            res


[<AbstractClass; Sealed>]
type YamlSerializer() =

    static member Deserialize (node:YamlNode, returnType:Type, [<Optional>] options:YamlSerializerOptions option): obj =
        let options = options |> Option.defaultValue YamlDefaults.options

        let serializer = YamlSerializerContext(options)
        try
            (serializer :> IYamlSerializer).Deserialize (returnType.Name, node, returnType)
        with
        | ex ->
            let rec findSerializerException (innerEx: Exception) =
                match innerEx with
                | :? YamlSerializerException -> innerEx
                | null -> ex
                | _ -> findSerializerException innerEx.InnerException

            let meaningfulEx = findSerializerException ex
            let path = String.Join(".", serializer.Contexts |> Seq.rev)
            let msg = $"Error while deserializing {path}: {meaningfulEx.Message}"
            YamlSerializerException(msg, meaningfulEx.InnerException) |> raise

    static member Deserialize<'T>(node:YamlNode, [<Optional>] options:YamlSerializerOptions option): 'T =
        YamlSerializer.Deserialize(node, typeof<'T>, options) :?> 'T
