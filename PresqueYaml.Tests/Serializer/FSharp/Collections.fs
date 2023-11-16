module MagnusOpera.PresqueYaml.Tests.Serializer.FSharpCollections

open MagnusOpera.PresqueYaml
open MagnusOpera.PresqueYaml.Serializer
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
    
    YamlSerializer.Deserialize<int list>(node, Defaults.options)
    |> should equal expected

// ####################################################################################################################

[<Test>]
let ``empty list conversion``() =
    let node = YamlNode.None
    
    YamlSerializer.Deserialize<string list>(node, Defaults.options)
    |> should be Empty

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
    
    YamlSerializer.Deserialize<Set<int>>(node, Defaults.options)
    |> should equal expected

// ####################################################################################################################

[<Test>]
let ``empty set conversion``() =
    let node = YamlNode.None
    
    YamlSerializer.Deserialize<Set<string>>(node, Defaults.options)
    |> should be Empty

// ####################################################################################################################

[<Test>]
let ``map conversion``() =
    let expected = Map [
        "toto", 42
        "titi", 666
    ]

    let node = YamlNode.Mapping (Map ["toto", YamlNode.Scalar "42"
                                      "titi", YamlNode.Scalar "666"])
    
    YamlSerializer.Deserialize<Map<string, int>>(node, Defaults.options)
    |> should equal expected

// ####################################################################################################################

[<Test>]
let ``empty map conversion``() =
    let node = YamlNode.None
    
    YamlSerializer.Deserialize<Map<string, int>>(node, Defaults.options)
    |> should be Empty
