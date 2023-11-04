module PresqueYaml.Tests.RepresentationModel.Scalar

open PresqueYaml.RepresentationModel
open NUnit.Framework
open FsUnit

// ####################################################################################################################

[<Test>]
let ``scalar only is valid``() =
    let expected = YamlNode.Scalar "toto"

    let yaml = "toto"
    yaml
    |> read
    |> should equal expected

// ####################################################################################################################

[<Test>]
let ``multiline scalar is valid``() =
    let expected = YamlNode.Scalar "toto titi"

    let yaml = "toto
titi"
    yaml
    |> read
    |> should equal expected

// ####################################################################################################################

[<Test>]
let ``scalar only with spaces is valid``() =
    let expected = YamlNode.Scalar "toto"

    let yaml = "toto     "
    yaml
    |> read
    |> should equal expected

// ####################################################################################################################
