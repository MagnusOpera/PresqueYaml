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

// [<Test>]
// let ``scalar override is valid``() =
//     let expected = YamlNode.Scalar "titi"

//     let yaml = "toto
// titi"
//     yaml
//     |> parse
//     |> should equal expected

// ####################################################################################################################

[<Test>]
let ``scalar only with spaces is valid``() =
    let expected = YamlNode.Scalar "toto"

    let yaml = "toto     "
    yaml
    |> read
    |> should equal expected

// ####################################################################################################################
