module MagnusOpera.PresqueYaml.Tests.Serializer.Nullable

open MagnusOpera.PresqueYaml
open MagnusOpera.PresqueYaml.Serializer
open NUnit.Framework
open FsUnit
open System

// ####################################################################################################################

[<Test>]
let ``nullable some conversion``() =
    let node = YamlNode.Scalar "42"
    
    YamlSerializer.Deserialize<Nullable<int>>(node, Defaults.options)
    |> should equal (Nullable<int>(42))

// ####################################################################################################################

[<Test>]
let ``nullable none conversion``() =
    let node = YamlNode.None
    
    YamlSerializer.Deserialize<Nullable<int>>(node, Defaults.options)
    |> should equal (Nullable<int>())
