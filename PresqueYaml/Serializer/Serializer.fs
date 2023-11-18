namespace MagnusOpera.PresqueYaml
open System
open MagnusOpera.PresqueYaml


type YamlSerializerContext(options:YamlSerializerOptions) =
    let getConverter (returnType:Type) =
        let factory = options.GetConverter returnType
        let converter = factory.CreateConverter(returnType)
        converter

    member val Contexts = System.Collections.Generic.Stack<string>()

    interface IYamlSerializer with
        member _.Options: YamlSerializerOptions =
            options

        member this.Default(returnType: Type): obj =
            let serializer = this :> IYamlSerializer
            let converter = getConverter returnType
            let defaultMethodInfo = converter.GetType() |> TypeHelpers.getDefault
            defaultMethodInfo.Invoke(converter, [| serializer.Options |])

        member this.Deserialize(context: string, node: YamlNode, returnType: Type): obj =
            this.Contexts.Push(context)
            let serializer = this :> IYamlSerializer
            let converter = getConverter returnType
            let readMethodInfo = converter.GetType() |> TypeHelpers.getRead
            let res = readMethodInfo.Invoke(converter, [| node; serializer |])
            this.Contexts.Pop() |> ignore
            res

        member this.Deserialize(context: string, node: YamlNode): 'T =
            let serializer = this :> IYamlSerializer
            serializer.Deserialize(context, node, typeof<'T>) :?> 'T


[<AbstractClass; Sealed>]
type YamlSerializer() =

    static member Deserialize (node:YamlNode, returnType:Type, options:YamlSerializerOptions): obj =
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

    static member Deserialize<'T>(node:YamlNode, options:YamlSerializerOptions): 'T =
        YamlSerializer.Deserialize(node, typeof<'T>, options) :?> 'T

    static member Deserialize<'T>(node:YamlNode): 'T =
        YamlSerializer.Deserialize(node, typeof<'T>, Defaults.options) :?> 'T
