module MagnusOpera.PresqueYaml.Tests.Serializer.YamlNodeValue

open MagnusOpera.PresqueYaml
open NUnit.Framework
open FsUnit


type Titi = {
    String: YamlNodeValue<string>
    Int: int
}

// ####################################################################################################################

[<Test>]
let ``yamlnode conversion``() =
    let node = YamlNode.Scalar "42"

    YamlSerializer.Deserialize<YamlNode option>(node, Defaults.options)
    |> should equal (Some node)

// ####################################################################################################################

[<Test>]
let ``none option record conversion``() =
    let expected = { Titi.String = YamlNodeValue.Undefined; Titi.Int = 42 }

    let node = YamlNode.Mapping (Map [ "Int", YamlNode.Scalar "42" ])

    YamlSerializer.Deserialize<Titi>(node, Defaults.options)
    |> should equal expected
