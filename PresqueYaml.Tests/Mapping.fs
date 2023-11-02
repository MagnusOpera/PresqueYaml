module PresqueYaml.Mapping.Tests

open PresqueYaml
open NUnit.Framework
open FsUnit

[<Test>]
let ``empty yaml is None`` () =
    let expected = YamlNode.None

    let yaml = ""
    yaml
    |> parse
    |> should equal expected

[<Test>]
let ``mapping only is valid``() =
    let expected =
        YamlNode.Mapping (Map [ "name", YamlNode.Scalar "John Doe"
                                "age", YamlNode.Scalar "42" ])

    let yaml = "name: John Doe
age: 42"

    yaml
    |> parse
    |> should equal expected


[<Test>]
let ``mapping only indented is valid``() =
    let expected =
        YamlNode.Mapping (Map [ "name", YamlNode.Scalar "John Doe"
                                "age", YamlNode.Scalar "42" ])

    let yaml = " name: John Doe
 age: 42"

    yaml
    |> parse
    |> should equal expected

[<Test>]
let ``mapping only with spaces is valid``() =
    let expected =
        YamlNode.Mapping (Map [ "name", YamlNode.Scalar "John Doe"
                                "age", YamlNode.Scalar "42" ])

    let yaml = " name: John Doe   
 age:   42   "

    yaml
    |> parse
    |> should equal expected



[<Test>]
let ``nested mappings are valid``() =
    let expected =
        YamlNode.Mapping (Map [ "name", YamlNode.Scalar "John Doe"
                                "age", YamlNode.Scalar "42"
                                "languages", YamlNode.Sequence ["F#"; "Python" ] ])

    let yaml = "name: John Doe
age: 42
languages:
  - F#
  - Python"

    yaml
    |> parse
    |> should equal expected


[<Test>]
let ``mapping override is allowed``() =
    let expected =
        YamlNode.Mapping (Map [ "name", YamlNode.Scalar "John Doe"
                                "age", YamlNode.Scalar "666"
                                "languages", YamlNode.Sequence ["F#"; "Python" ] ])

    let yaml = "name: John Doe
age: 42
languages:
  - F#
  - Python
age: 666
"

    yaml
    |> parse
    |> should equal expected


[<Test>]
let ``mapping override null is allowed``() =
    let expected =
        YamlNode.Mapping (Map [ "name", YamlNode.Scalar "John Doe"
                                "age", YamlNode.None ])

    let yaml = "name: John Doe
age: 42
age:"

    yaml
    |> parse
    |> should equal expected
