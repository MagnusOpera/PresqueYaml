module PresqueYaml.Tests.RepresentationModel.Sequence

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
    |> read
    |> should equal expected

// ####################################################################################################################

[<Test>]
let ``values in sequence are trimmed``() =
    let expected = YamlNode.Sequence ["toto"; "titi"]

    let yaml = "-   toto
- titi   "

    yaml
    |> read
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
    |> read
    |> should equal expected

// ####################################################################################################################

[<Test>]
let ``type mismatch in list is error``() =
    let yaml = "- toto
-titi"

    (fun () -> yaml |> read |> ignore)
    |> should (throwWithMessage "Unexpected data type (line 2)") typeof<System.Exception>

// ####################################################################################################################

[<Test>]
let ``type mismatch scalar first in list is error``() =
    let yaml = "users:
  -toto
  - titi"

    (fun () -> yaml |> read |> ignore)
    |> should (throwWithMessage "Unexpected data type (line 2)") typeof<System.Exception>

// ####################################################################################################################