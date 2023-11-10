module PresqueYaml.Tests.Serializer.Collections

open System.Collections.Generic
open PresqueYaml
open PresqueYaml.Serializer
open NUnit.Framework
open FsUnit

// ####################################################################################################################

[<Test>]
let ``list conversion``() =
    let expected = List<int> [ 1; 2; 3; 4]

    let node = YamlNode.Sequence [
        YamlNode.Scalar "1"
        YamlNode.Scalar "2"
        YamlNode.Scalar "3"
        YamlNode.Scalar "4"
    ]
    
    YamlSerializer.Deserialize<List<int>>(node, PresqueYaml.Defaults.options)
    |> should equal expected

// ####################################################################################################################

[<Test>]
let ``empty list conversion``() =
    let node = YamlNode.None
    
    YamlSerializer.Deserialize<List<string>>(node, PresqueYaml.Defaults.options)
    |> should be Empty

// ####################################################################################################################

[<Test>]
let ``array conversion``() =
    let expected = [| 1; 2; 3; 4 |]

    let node = YamlNode.Sequence [
        YamlNode.Scalar "1"
        YamlNode.Scalar "2"
        YamlNode.Scalar "3"
        YamlNode.Scalar "4"
    ]
    
    YamlSerializer.Deserialize<int[]>(node, PresqueYaml.Defaults.options)
    |> should equal expected

// ####################################################################################################################

[<Test>]
let ``empty array conversion``() =
    let node = YamlNode.None
    
    YamlSerializer.Deserialize<string[]>(node, PresqueYaml.Defaults.options)
    |> should be Empty

// ####################################################################################################################

[<Test>]
let ``dictionary conversion``() =
    let expected = Dictionary<string, int> [
        KeyValuePair("toto", 42)
        KeyValuePair("titi", 666)
    ]

    let node = YamlNode.Mapping (Map ["toto", YamlNode.Scalar "42"
                                      "titi", YamlNode.Scalar "666"])
    
    YamlSerializer.Deserialize<Dictionary<string, int>>(node, PresqueYaml.Defaults.options)
    |> should equal expected

// ####################################################################################################################

[<Test>]
let ``empty dictionary conversion``() =
    let node = YamlNode.None
    
    YamlSerializer.Deserialize<Dictionary<string, int>>(node, PresqueYaml.Defaults.options)
    |> should be Empty
