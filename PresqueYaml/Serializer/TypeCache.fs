module TypeCache
open FSharp.Reflection
open System.Collections.Generic
open System

// Have to use concurrentdictionary here because dictionaries thrown on non-locked access:
(* Error Message:
    System.InvalidOperationException : Operations that change non-concurrent collections must have exclusive access. A concurrent update was performed on this collection and corrupted its state. The collection's state is no longer correct.
    Stack Trace:
        at System.Collections.Generic.Dictionary`2.TryInsert(TKey key, TValue value, InsertionBehavior behavior) *)
type Dict<'a, 'b> = System.Collections.Concurrent.ConcurrentDictionary<'a, 'b>

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

/// cached access to FSharpType.* and System.Type to prevent repeated access to reflection members
let getKind =
    let cache = Dict<System.Type, TypeKind>()
    let fslistTy = typedefof<_ list>
    let fssetTy = typedefof<Set<_>>
    let fsmapTy = typedefof<Map<_, _>>

    let listTy = typedefof<List<_>>
    let dictionaryTy = typedefof<Dictionary<_, _>>
    let nullableTy = typedefof<Nullable<_>>

    fun (ty: System.Type) ->
        cache.GetOrAdd(
            ty,
            fun ty ->
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
        )

let isFsUnion ty =
    getKind ty = TypeKind.FsUnion

let isFsRecord ty =
    getKind ty = TypeKind.FsRecord

let isFsList ty =
    getKind ty = TypeKind.FsList

let isFsSet ty =
    getKind ty = TypeKind.FsSet

let isFsMap ty =
    getKind ty = TypeKind.FsMap

let isFsTuple ty =
    getKind ty = TypeKind.FsTuple

let isList ty =
    getKind ty = TypeKind.List

let isArray ty =
    getKind ty = TypeKind.Array

let isDictionary ty =
    getKind ty = TypeKind.Dictionary

let isNullable ty =
    getKind ty = TypeKind.Nullable
