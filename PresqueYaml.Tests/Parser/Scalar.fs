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
    let expected = YamlNode.Scalar "toto titi"

    let yaml = "
toto
titi"

    yaml
    |> Parser.read
    |> should equal expected

// ####################################################################################################################

[<Test>]
let ``multiline folded scalar is error``() =
    let yaml = "|
toto
titi"

    (fun () -> yaml |> Parser.read |> ignore)
    |> should (throwWithMessage "Indentation error (line 2, column 1)") typeof<System.Exception>

// ####################################################################################################################

[<Test>]
let ``multiline literal scalar is error``() =
    let yaml = ">
toto
titi"

    (fun () -> yaml |> Parser.read |> ignore)
    |> should (throwWithMessage "Indentation error (line 2, column 1)") typeof<System.Exception>

// ####################################################################################################################

[<Test>]
let ``scalar only with spaces is valid``() =
    let expected = YamlNode.Scalar "toto"

    let yaml = "toto     "
    yaml
    |> Parser.read
    |> should equal expected

// ####################################################################################################################

[<Test>]
let ``multiline folded scalar in mapping is valid``() =
    let expected = YamlNode.Mapping (Map [ "toto", YamlNode.Scalar "John Doe"])

    let yaml = "
toto: >
  John
  Doe  "

    yaml
    |> Parser.read
    |> should equal expected


// ####################################################################################################################

[<Test>]
let ``multiline literal scalar must be indented``() =
    let expected = YamlNode.Mapping (Map [ "toto", YamlNode.Scalar "John Doe"])

    let yaml = "
toto:  John
       Doe  "

    yaml
    |> Parser.read
    |> should equal expected
