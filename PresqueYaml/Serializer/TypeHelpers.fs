module private TypeHelpers
open System.Reflection
open FSharp.Reflection
open System.Collections.Generic
open System
open MagnusOpera.PresqueYaml

type TypeKind =
    | FsRecord = 0
    | FsUnion = 1
    | FsList = 2
    | FsSet = 3
    | FsMap = 4
    | FsTuple = 5
    | FsUnit = 6
    | List = 50
    | Array = 51
    | Dictionary = 52
    | Nullable = 53
    | Other = 100

let private fslistTy = typedefof<_ list>
let private fssetTy = typedefof<Set<_>>
let private fsmapTy = typedefof<Map<_, _>>
let private listTy = typedefof<List<_>>
let private dictionaryTy = typedefof<Dictionary<_, _>>
let private nullableTy = typedefof<Nullable<_>>
let private fsunit = typeof<unit>

let private matchType (ty: Type) =
    if ty.IsGenericType && ty.GetGenericTypeDefinition() = fslistTy then TypeKind.FsList
    elif ty.IsGenericType && ty.GetGenericTypeDefinition() = fssetTy then TypeKind.FsSet
    elif ty.IsGenericType && ty.GetGenericTypeDefinition() = fsmapTy then TypeKind.FsMap
    elif ty.IsGenericType && ty.GetGenericTypeDefinition() = listTy then TypeKind.List
    elif ty.IsGenericType && ty.GetGenericTypeDefinition() = dictionaryTy then TypeKind.Dictionary
    elif ty.IsGenericType && ty.GetGenericTypeDefinition() = nullableTy then TypeKind.Nullable
    elif FSharpType.IsTuple(ty) then TypeKind.FsTuple
    elif FSharpType.IsUnion(ty, true) then TypeKind.FsUnion
    elif FSharpType.IsRecord(ty, true) then TypeKind.FsRecord
    elif ty = fsunit then TypeKind.FsUnit
    elif ty.IsArray then TypeKind.Array
    else TypeKind.Other

let private readMethod (ty: Type) = ty.GetMethod("Read")
let private defaultMethod (ty: Type) = ty.GetMethod("Default")

let private cache = System.Collections.Concurrent.ConcurrentDictionary<System.Type, TypeKind>()
let getKind ty = cache.GetOrAdd(ty, matchType)

let private readCache = System.Collections.Concurrent.ConcurrentDictionary<System.Type, MethodInfo>()
let getRead ty = readCache.GetOrAdd(ty, readMethod)

let private defaultCache = System.Collections.Concurrent.ConcurrentDictionary<System.Type, MethodInfo>()
let getDefault ty = defaultCache.GetOrAdd(ty, defaultMethod)


let nrtContext = NullabilityInfoContext()

let getRequired (ty: Type) (nrtInfo: NullabilityInfo): bool =
    match nrtInfo.ReadState with
    | NullabilityState.Nullable -> false
    | NullabilityState.NotNull -> true
    | _ ->
        // F# type ?
        match ty.GetCustomAttribute(typeof<CompilationMappingAttribute>) with
        | null -> false
        | _ ->
            // F# null allowed ?
            match ty.GetCustomAttribute(typeof<AllowNullLiteralAttribute>) with
            | null ->
                // provide some nullability for few F# types
                if ty.IsGenericType then
                    let gen = ty.GetGenericTypeDefinition()
                    if gen = typedefof<option<_>> then false
                    elif gen = typedefof<voption<_>> then false
                    elif gen = typedefof<YamlNodeValue<_>> then false
                    elif gen = typedefof<list<_>> then false
                    elif gen = typedefof<Set<_>> then false
                    elif gen = typedefof<Map<string,_>> then false
                    else true
                else
                    true
            | _ -> false

let getPropertyRequired (propInfo: PropertyInfo) =
    getRequired propInfo.PropertyType (nrtContext.Create(propInfo))

let getParameterRequired (paramInfo: ParameterInfo) =
    getRequired paramInfo.ParameterType (nrtContext.Create(paramInfo))
