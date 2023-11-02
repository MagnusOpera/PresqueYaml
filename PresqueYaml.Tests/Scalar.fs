module PresqueYaml.Scalar.Tests

open PresqueYaml
open NUnit.Framework
open FsUnit

[<Test>]
let ``scalar only is valid``() =
    let expected = YamlNode.Scalar "toto"

    let yaml = "toto"
    yaml
    |> parse
    |> should equal expected

[<Test>]
let ``scalar only indented is valid``() =
    let expected = YamlNode.Scalar "toto"

    let yaml = "  toto"
    yaml
    |> parse
    |> should equal expected

[<Test>]
let ``scalar only with spaces is valid``() =
    let expected = YamlNode.Scalar "toto"

    let yaml = "  toto     "
    yaml
    |> parse
    |> should equal expected

[<Test>]
let ``multiple scalars is error``() =
    let yaml = "toto
titi"

    (fun () -> yaml |> parse |> ignore)
    |> should (throwWithMessage "Unexpected elements line 2") typeof<System.Exception>
