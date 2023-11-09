module TypeCache
open FSharp.Reflection
open System.Collections.Generic
open System

type TypeKind =
    | FsRecord = 0
    | FsUnion = 1
    | FsList = 2
    | FsSet = 3
    | FsMap = 4
    | FsTuple = 5
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
    elif ty.IsArray then TypeKind.Array
    else TypeKind.Other


let private cache = System.Collections.Concurrent.ConcurrentDictionary<System.Type, TypeKind>()
let getKind ty = cache.GetOrAdd(ty, matchType)
