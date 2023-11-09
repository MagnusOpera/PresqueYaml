module PresqueYaml.Tests.Parser.Scalar

open PresqueYaml
open NUnit.Framework
open FsUnit

// ####################################################################################################################

[<Test>]
let ``scalar only is valid``() =
    let expected = YamlNode.Scalar "toto"

    let yaml = "toto"
    yaml
    |> Parser.read
    |> should equal expected

// ####################################################################################################################

[<Test>]
let ``multiline scalar is valid``() =
    let expected = YamlNode.Scalar "toto\ntiti"

    let yaml = "toto
titi"
    yaml
    |> Parser.read
    |> should equal expected

// ####################################################################################################################

[<Test>]
let ``scalar only with spaces is valid``() =
    let expected = YamlNode.Scalar "toto"

    let yaml = "toto     "
    yaml
    |> Parser.read
    |> should equal expected

// ####################################################################################################################
