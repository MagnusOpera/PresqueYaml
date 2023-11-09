module PresqueYaml.Serializer.Helpers
open System
open Microsoft.FSharp.Reflection

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

