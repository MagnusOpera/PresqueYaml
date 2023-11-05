module PresqueYaml.Tests.Model.Sequence

open PresqueYaml.Model
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
    |> read
    |> should equal expected

// ####################################################################################################################

[<Test>]
let ``compact sequence in sequence is valid``() =
    let expected =
        YamlNode.Sequence [ YamlNode.Sequence [ YamlNode.Scalar "John Doe"
                                                YamlNode.Scalar "Jane Doe" ]
                            YamlNode.Sequence [ YamlNode.Scalar "F#"
                                                YamlNode.Scalar "Python" ] ]

    let yaml = "
- -    John Doe
  -   Jane Doe
- - F#
  -    Python
"

    yaml
    |> read
    |> should equal expected

// ####################################################################################################################

[<Test>]
let ``type mismatch in list is error``() =
    let yaml = "- toto
-titi"

    (fun () -> yaml |> read |> ignore)
    |> should (throwWithMessage "Type mismatch (line 2, column 1)") typeof<System.Exception>

// ####################################################################################################################

[<Test>]
let ``type mismatch scalar first in list is error``() =
    let yaml = "users:
  -toto
  - titi"

    (fun () -> yaml |> read |> ignore)
    |> should (throwWithMessage "Type mismatch (line 3, column 3)") typeof<System.Exception>

// ####################################################################################################################
