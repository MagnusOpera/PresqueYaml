module PresqueYaml.Mapping.Tests

open PresqueYaml
open NUnit.Framework
open FsUnit

// ####################################################################################################################

[<Test>]
let ``empty yaml is None and valid`` () =
    let expected = YamlNode.None

    let yaml = ""
    yaml
    |> read
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
    |> read
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
    |> read
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
    |> read
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
    |> read
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
    |> read
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
    |> read
    |> should equal expected

// ####################################################################################################################

[<Test>]
let ``mapping type mismatch is error``() =
    let yaml = "users: 42
- toto"

    (fun () -> yaml |> read |> ignore)
    |> should (throwWithMessage "Unexpected data type (line 2)") typeof<System.Exception>

// ####################################################################################################################
