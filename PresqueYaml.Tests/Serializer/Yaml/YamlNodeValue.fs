module MagnusOpera.PresqueYaml.Tests.Serializer.YamlNodeValue

open MagnusOpera.PresqueYaml
open NUnit.Framework
open FsUnit


type Titi = {
    String: YamlNodeValue<int>
    StringUndefined: YamlNodeValue<int>
    StringNone: YamlNodeValue<int>
}

// ####################################################################################################################

[<Test>]
let ``yamlnode conversion``() =
    let node = YamlNode.Scalar "42"

    YamlSerializer.Deserialize<YamlNode>(node)
    |> should equal node

// ####################################################################################################################

[<Test>]
let ``yamlnode option conversion``() =
    let node = YamlNode.Scalar "42"

    YamlSerializer.Deserialize<YamlNode option>(node)
    |> should equal (Some node)

// ####################################################################################################################

[<Test>]
let ``YamlNodeValue conversion``() =
    let expected = { Titi.String = YamlNodeValue.Value 42
                     Titi.StringNone = YamlNodeValue.None
                     Titi.StringUndefined = YamlNodeValue.Undefined }

    let node = YamlNode.Mapping (Map [ "String", YamlNode.Scalar "42"
                                       "StringNone", YamlNode.None ])

    YamlSerializer.Deserialize<Titi>(node)
    |> should equal expected
