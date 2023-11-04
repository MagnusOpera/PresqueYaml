module PresqueYaml.Tests.RepresentationModel.Wellformed

open PresqueYaml.RepresentationModel
open NUnit.Framework
open FsUnit

// ####################################################################################################################

[<Test>]
let ``empty lines are ignored``() =
    let expected =
        YamlNode.Mapping (Map [ "user1", YamlNode.Mapping (Map [ "name", YamlNode.Scalar "John Doe"
                                                                 "age", YamlNode.None ] )
                                "user2", YamlNode.Mapping (Map [ "name", YamlNode.Scalar "Jane Doe"
                                                                 "age", YamlNode.Scalar "42"
                                                                 "languages", YamlNode.Sequence [ YamlNode.Scalar "F#"
                                                                                                  YamlNode.Scalar "Python" ] ] ) ])

    let yaml = "

#     user 1
user1:

  name: John Doe

  age:


   # this is a comment
user2:
  name: Jane Doe

  age: 42
  languages:
    -    F#
    -   Python


"

    yaml
    |> read
    |> should equal expected

