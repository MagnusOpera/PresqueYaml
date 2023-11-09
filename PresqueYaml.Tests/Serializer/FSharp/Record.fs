module PresqueYaml.Tests.Serializer.Record

open PresqueYaml.Model
open PresqueYaml.Serializer
open NUnit.Framework
open FsUnit



type Toto = {
    String: string
    StringOption: string option
    // StringVOption: string voption
    Int: int
    IntOption: int option
    // IntVOption: int voption
}
// ####################################################################################################################

[<Test>]
let ``record conversion``() =
    let expected = {
        String = "tralala"
        StringOption = None
        // StringVOption = ValueSome "pouet"
        Int = 42
        IntOption = Some 666
        // IntVOption = ValueSome -1
    }

    let node = YamlNode.Mapping (Map ["String", YamlNode.Scalar "tralala"
                                      "StringVOption", YamlNode.Scalar "pouet"
                                      "Int", YamlNode.Scalar "42"
                                      "IntOption", YamlNode.Scalar "666"
                                      "IntVOption", YamlNode.Scalar "-1" ])
    
    YamlSerializer.Deserialize<Toto>(node, PresqueYaml.Defaults.options)
    |> should equal expected


// ####################################################################################################################

[<Test>]
let ``option record conversion``() =
    let expected = Some {
        String = "tralala"
        StringOption = None
        // StringVOption = ValueSome "pouet"
        Int = 42
        IntOption = Some 666
        // IntVOption = ValueSome -1
    }

    let node = YamlNode.Mapping (Map ["String", YamlNode.Scalar "tralala"
                                      "StringVOption", YamlNode.Scalar "pouet"
                                      "Int", YamlNode.Scalar "42"
                                      "IntOption", YamlNode.Scalar "666"
                                      "IntVOption", YamlNode.Scalar "-1" ])
    
    YamlSerializer.Deserialize<Toto option>(node, PresqueYaml.Defaults.options)
    |> should equal expected
