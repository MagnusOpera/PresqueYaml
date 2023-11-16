module PresqueYaml.Tests.Parser.Sequence

open PresqueYaml
open NUnit.Framework
open FsUnit

// ####################################################################################################################

[<Test>]
let ``sequence only is valid``() =
    let expected = YamlNode.Sequence [YamlNode.Scalar "toto"; YamlNode.Scalar "titi"]

    let yaml = "- toto
- titi"

    yaml
    |> YamlParser.read
    |> should equal expected

// ####################################################################################################################

[<Test>]
let ``compact sequence is valid``() =
    let expected = YamlNode.Mapping (Map [ "users", YamlNode.Sequence [YamlNode.Scalar "toto"; YamlNode.Scalar "titi"] ])

    let yaml = "users: [ toto, titi ]"

    yaml
    |> YamlParser.read
    |> should equal expected

// ####################################################################################################################

[<Test>]
let ``values in sequence are trimmed``() =
    let expected = YamlNode.Sequence [YamlNode.Scalar "toto"; YamlNode.Scalar "titi"]

    let yaml = "-   toto
- titi   "

    yaml
    |> YamlParser.read
    |> should equal expected

// ####################################################################################################################

[<Test>]
let ``sequence in mapping valid``() =
    let expected =
        YamlNode.Mapping (Map [ "name", YamlNode.Scalar "John Doe"
                                "age", YamlNode.Scalar "42"
                                "languages", YamlNode.Sequence [YamlNode.Scalar "F#"; YamlNode.Scalar "Python" ] ])

    let yaml = "name: John Doe
age: 42
languages:
  - F#
  - Python"

    yaml
    |> YamlParser.read
    |> should equal expected

// ####################################################################################################################

[<Test>]
let ``mapping in sequence is valid``() =
    let expected =
        YamlNode.Sequence [ YamlNode.Mapping( Map [ "name", YamlNode.Scalar "John Doe"
                                                    "age", YamlNode.Scalar "42" ] )
                            YamlNode.Mapping( Map [ "name", YamlNode.Scalar "Jane Doe"
                                                    "languages", YamlNode.Sequence [ YamlNode.Scalar "F#"
                                                                                     YamlNode.Scalar "Python" ] ]) ]

    let yaml = "- name: John Doe
  age: 42
- name: Jane Doe
  languages:
    - F#
    - Python"

    yaml
    |> YamlParser.read
    |> should equal expected

// ####################################################################################################################

[<Test>]
let ``nested inline sequence is valid``() =
    let expected =
        YamlNode.Sequence [ YamlNode.Sequence [ YamlNode.Scalar "John Doe"
                                                YamlNode.Scalar "Jane Doe" ]
                            YamlNode.Sequence [ YamlNode.Scalar "F#"
                                                YamlNode.Scalar "Python" ] ]

    let yaml = "
-
  -    John Doe
  -   Jane Doe
- - F#
  -    Python


"

    yaml
    |> YamlParser.read
    |> should equal expected

// ####################################################################################################################

[<Test>]
let ``sequence fails if no spaces after hyphen``() =
    let yaml = "- toto
-titi"

    (fun () -> yaml |> YamlParser.read |> ignore)
    |> should (throwWithMessage "Expecting sequence (line 2, column 1)") typeof<System.Exception>
