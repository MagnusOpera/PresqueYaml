module PresqueYaml.Sequence.Tests

open PresqueYaml
open NUnit.Framework
open FsUnit

// ####################################################################################################################

[<Test>]
let ``sequence only is valid``() =
    let expected = YamlNode.Sequence ["toto"; "titi"]

    let yaml = "- toto
- titi"

    yaml
    |> parse
    |> should equal expected

// ####################################################################################################################

[<Test>]
let ``values in sequence are trimmed``() =
    let expected = YamlNode.Sequence ["toto"; "titi"]

    let yaml = "-   toto
- titi   "

    yaml
    |> parse
    |> should equal expected

// ####################################################################################################################

[<Test>]
let ``sequence in mapping valid``() =
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

// ####################################################################################################################

[<Test>]
let ``type mismatch in list is error``() =
    let yaml = "- toto
-titi"

    (fun () -> yaml |> parse |> ignore)
    |> should (throwWithMessage "Unexpected data type (line 2)") typeof<System.Exception>

// ####################################################################################################################

[<Test>]
let ``type mismatch scalar first in list is error``() =
    let yaml = "users:
  -toto
  - titi"

    (fun () -> yaml |> parse |> ignore)
    |> should (throwWithMessage "Unexpected data type (line 2)") typeof<System.Exception>

// ####################################################################################################################
