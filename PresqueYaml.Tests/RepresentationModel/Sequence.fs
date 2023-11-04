module PresqueYaml.Tests.RepresentationModel.Sequence

open PresqueYaml.RepresentationModel
open NUnit.Framework
open FsUnit

// ####################################################################################################################

[<Test>]
let ``sequence only is valid``() =
    let expected = YamlNode.Sequence [YamlNode.Scalar "toto"; YamlNode.Scalar "titi"]

    let yaml = "- toto
- titi"

    yaml
    |> read
    |> should equal expected

// ####################################################################################################################

[<Test>]
let ``values in sequence are trimmed``() =
    let expected = YamlNode.Sequence [YamlNode.Scalar "toto"; YamlNode.Scalar "titi"]

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
                                "languages", YamlNode.Sequence [YamlNode.Scalar "F#"; YamlNode.Scalar "Python" ] ])

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
    |> should (throwWithMessage "Type mismatch (line 2)") typeof<System.Exception>

// ####################################################################################################################

[<Test>]
let ``type mismatch scalar first in list is error``() =
    let yaml = "users:
  -toto
  - titi"

    (fun () -> yaml |> read |> ignore)
    |> should (throwWithMessage "Type mismatch (line 3)") typeof<System.Exception>

// ####################################################################################################################
