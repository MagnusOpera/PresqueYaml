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
    |> YamlParser.read
    |> should equal expected

// ####################################################################################################################

[<Test>]
let ``multiline scalar is valid and folded is default``() =
    let expected = YamlNode.Scalar "toto titi"

    let yaml = "
toto
titi"

    yaml
    |> YamlParser.read
    |> should equal expected

// ####################################################################################################################

[<Test>]
let ``multiline folded scalar is valid``() =
    let expected = YamlNode.Scalar "toto\n  titi"

    let yaml = "|
  toto
    titi"

    yaml
    |> YamlParser.read
    |> should equal expected

// ####################################################################################################################

[<Test>]
let ``multiline literal scalar is valid``() =
    let expected = YamlNode.Scalar "toto   titi"

    let yaml = ">
 toto
   titi"

    yaml
    |> YamlParser.read
    |> should equal expected

// ####################################################################################################################

[<Test>]
let ``scalar only with spaces is valid``() =
    let expected = YamlNode.Scalar "toto"

    let yaml = "  toto     "
    yaml
    |> YamlParser.read
    |> should equal expected

// ####################################################################################################################

[<Test>]
let ``multiline folded scalar in mapping is valid and relative indentation is preserved``() =
    let expected = YamlNode.Mapping (Map [ "toto", YamlNode.Scalar "John  Doe"])

    let yaml = "
toto: >
   John
    Doe  "

    yaml
    |> YamlParser.read
    |> should equal expected


// ####################################################################################################################

[<Test>]
let ``compact literal scalar is valid in mapping``() =
    let expected = YamlNode.Mapping (Map [ "toto", YamlNode.Scalar "John"])

    let yaml = "
toto:  John"

    yaml
    |> YamlParser.read
    |> should equal expected
