module PresqueYaml.Tests.Parser.Mapping

open PresqueYaml
open NUnit.Framework
open FsUnit

// ####################################################################################################################

[<Test>]
let ``empty yaml is None and valid`` () =
    let expected = YamlNode.None

    let yaml = ""
    yaml
    |> YamlParser.read
    |> should equal expected

// ####################################################################################################################

[<Test>]
let ``mapping only is valid``() =
    let expected =
        YamlNode.Mapping (Map [ "name", YamlNode.Scalar "John Doe"
                                "age", YamlNode.Scalar "42" ])

    let yaml = "name: John Doe
age: 42"

    yaml
    |> YamlParser.read
    |> should equal expected

// ####################################################################################################################

[<Test>]
let ``mapping values are trimmed``() =
    let expected =
        YamlNode.Mapping (Map [ "name", YamlNode.Scalar "John Doe"
                                "age", YamlNode.Scalar "42" ])

    let yaml = "name: John Doe
age:   42   "

    yaml
    |> YamlParser.read
    |> should equal expected

// ####################################################################################################################

[<Test>]
let ``nested mappings indent is valid``() =
    let expected =
        YamlNode.Mapping (Map [ "user", YamlNode.Mapping (Map [ "name", YamlNode.Scalar "John Doe"
                                                                "age", YamlNode.Scalar "42" ] ) ])

    let yaml = "user:
  name: John Doe
  age: 42"

    yaml
    |> YamlParser.read
    |> should equal expected

// ####################################################################################################################

[<Test>]
let ``nested mappings dedent is valid``() =
    let expected =
        YamlNode.Mapping (Map [ "user1", YamlNode.Mapping (Map [ "name", YamlNode.Scalar "John Doe"
                                                                 "age", YamlNode.None ] )
                                "user2", YamlNode.Mapping (Map [ "name", YamlNode.Scalar "Jane Doe"
                                                                 "age", YamlNode.Scalar "42" ] ) ])

    let yaml = "user1:
  name: John Doe
  age:
user2:
  name: Jane Doe
  age: 42"

    yaml
    |> YamlParser.read
    |> should equal expected

// ####################################################################################################################

[<Test>]
let ``inline nested mapping is valid``() =
    let expected =
        YamlNode.Mapping (Map [ "user1", YamlNode.Mapping (Map [ "name", YamlNode.Scalar "John Doe"
                                                                 "age", YamlNode.None ] )
                                "user2", YamlNode.Mapping (Map [ "name", YamlNode.Scalar "Jane Doe"
                                                                 "age", YamlNode.Scalar "42" ] ) ])

    let yaml = "
user1: name: John Doe
       age:
user2:
  name: Jane Doe
  age: 42"

    yaml
    |> YamlParser.read
    |> should equal expected

// ####################################################################################################################

[<Test>]
let ``mapping override is allowed``() =
    let expected =
        YamlNode.Mapping (Map [ "name", YamlNode.Scalar "John Doe"
                                "age", YamlNode.Scalar "666" ])

    let yaml = "name: John Doe
age: 42
age: 666
"

    yaml
    |> YamlParser.read
    |> should equal expected

// ####################################################################################################################

[<Test>]
let ``mapping override null is allowed``() =
    let expected =
        YamlNode.Mapping (Map [ "name", YamlNode.Scalar "John Doe"
                                "age", YamlNode.None ])

    let yaml = "name: John Doe
age: 42
age:"

    yaml
    |> YamlParser.read
    |> should equal expected

// ####################################################################################################################

[<Test>]
let ``mapping value override must be on same line``() =
    let expected =
        YamlNode.Mapping (Map [ "user", YamlNode.Mapping (Map ["name", YamlNode.Scalar "toto\ntiti"]) ])

    let yaml = "
user:
  name: |
   toto
   titi

"

    yaml
    |> YamlParser.read
    |> should equal expected

// ####################################################################################################################

