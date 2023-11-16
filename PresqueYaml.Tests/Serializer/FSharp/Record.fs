module MagnusOpera.PresqueYaml.Tests.Serializer.Record

open MagnusOpera.PresqueYaml
open MagnusOpera.PresqueYaml.Converters
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

type Titi = {
    String: YamlNodeValue<string>
    Int: int
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

    YamlSerializer.Deserialize<Toto>(node, Defaults.options)
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

    YamlSerializer.Deserialize<Toto option>(node, Defaults.options)
    |> should equal expected

// ####################################################################################################################

[<Test>]
let ``none option record conversion``() =
    let expected = { Titi.String = YamlNodeValue.Undefined; Titi.Int = 42 }

    let node = YamlNode.Mapping (Map [ "Int", YamlNode.Scalar "42" ])

    YamlSerializer.Deserialize<Titi>(node, Defaults.options)
    |> should equal expected
