module PresqueYaml.Tests.RepresentationModel.Wellformed

open PresqueYaml.RepresentationModel
open NUnit.Framework
open FsUnit

// ####################################################################################################################

[<Test>]
let ``empty lines are ignored``() =
    let expected =
        YamlNode.Mapping (Map [ "user1", YamlNode.Mapping (Map [ "name", YamlNode.Scalar "John Doe"
                                                                 "age", YamlNode.None
                                                                 "comment", YamlNode.Scalar "this is a comment\n😃"
                                                                 "languages", YamlNode.Sequence [ YamlNode.Scalar "Python"
                                                                                                  YamlNode.None
                                                                                                  YamlNode.Scalar "F#" ] ] )
                                "user2", YamlNode.Mapping (Map [ "first name", YamlNode.Scalar "J a n e "
                                                                 "last name", YamlNode.Scalar "Doe"
                                                                 "age", YamlNode.Scalar "42"
                                                                 "languages", YamlNode.Sequence [ YamlNode.Scalar "F# |> ❤️"
                                                                                                  YamlNode.Scalar "Python" ] ] ) ])

    let yaml = "

#     user 1
user1:

  name: John Doe

  age:
  comment: this is a comment\\n😃
  languages: [ Python,, F# ]


   # this is a comment
user2:
  'first name': 'J a n e '
  'last name':   Doe

  age: 42
  languages:
    - F# |> ❤️
    -   Python


"

    yaml
    |> read
    |> should equal expected

