module PresqueYaml.Tests.Serializer.FSharpCollections

open PresqueYaml
open PresqueYaml.Serializer
open NUnit.Framework
open FsUnit

// ####################################################################################################################

[<Test>]
let ``list conversion``() =
    let expected = [ 1; 2; 3; 4]

    let node = YamlNode.Sequence [
        YamlNode.Scalar "1"
        YamlNode.Scalar "2"
        YamlNode.Scalar "3"
        YamlNode.Scalar "4"
    ]
    
    YamlSerializer.Deserialize<int list>(node, PresqueYaml.Defaults.options)
    |> should equal expected

// ####################################################################################################################

[<Test>]
let ``set conversion``() =
    let expected = Set [ 4; 2; 1; 3]

    let node = YamlNode.Sequence [
        YamlNode.Scalar "1"
        YamlNode.Scalar "3"
        YamlNode.Scalar "2"
        YamlNode.Scalar "1"
        YamlNode.Scalar "4"
    ]
    
    YamlSerializer.Deserialize<Set<int>>(node, PresqueYaml.Defaults.options)
    |> should equal expected

// ####################################################################################################################

[<Test>]
let ``map conversion``() =
    let expected = Map [
        "toto", 42
        "titi", 666
    ]

    let node = YamlNode.Mapping (Map ["toto", YamlNode.Scalar "42"
                                      "titi", YamlNode.Scalar "666"])
    
    YamlSerializer.Deserialize<Map<string, int>>(node, PresqueYaml.Defaults.options)
    |> should equal expected
