module PresqueYaml.Tests.Serializer.Convertible

open PresqueYaml.Model
open PresqueYaml.Serializer
open NUnit.Framework
open FsUnit

// ####################################################################################################################

[<Test>]
let ``short conversion``() =
    let node = YamlNode.Scalar "42"
    
    YamlSerializer.Deserialize<System.Int16>(node, PresqueYaml.Mappers.Defaults.defaultOptions)
    |> should equal 42s

// ####################################################################################################################

[<Test>]
let ``ushort conversion``() =
    let node = YamlNode.Scalar "42"
    
    YamlSerializer.Deserialize<System.UInt16>(node, PresqueYaml.Mappers.Defaults.defaultOptions)
    |> should equal 42us

// ####################################################################################################################

[<Test>]
let ``int conversion``() =
    let node = YamlNode.Scalar "42"
    
    YamlSerializer.Deserialize<System.Int32>(node, PresqueYaml.Mappers.Defaults.defaultOptions)
    |> should equal 42

// ####################################################################################################################

[<Test>]
let ``uint conversion``() =
    let node = YamlNode.Scalar "42"
    
    YamlSerializer.Deserialize<System.UInt32>(node, PresqueYaml.Mappers.Defaults.defaultOptions)
    |> should equal 42u

// ####################################################################################################################

[<Test>]
let ``long conversion``() =
    let node = YamlNode.Scalar "42"
    
    YamlSerializer.Deserialize<System.Int64>(node, PresqueYaml.Mappers.Defaults.defaultOptions)
    |> should equal 42L

// ####################################################################################################################

[<Test>]
let ``ulong conversion``() =
    let node = YamlNode.Scalar "42"
    
    YamlSerializer.Deserialize<System.UInt64>(node, PresqueYaml.Mappers.Defaults.defaultOptions)
    |> should equal 42UL

// ####################################################################################################################

[<Test>]
let ``string conversion``() =
    let node = YamlNode.Scalar "42"
    
    YamlSerializer.Deserialize<string>(node, PresqueYaml.Mappers.Defaults.defaultOptions)
    |> should equal "42"

// ####################################################################################################################

[<Test>]
let ``char conversion``() =
    let node = YamlNode.Scalar "A"
    
    YamlSerializer.Deserialize<char>(node, PresqueYaml.Mappers.Defaults.defaultOptions)
    |> should equal 'A'


// ####################################################################################################################

[<Test>]
let ``byte conversion``() =
    let node = YamlNode.Scalar "A"
    
    YamlSerializer.Deserialize<char>(node, PresqueYaml.Mappers.Defaults.defaultOptions)
    |> should equal 65uy
