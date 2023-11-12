module PresqueYaml.Tests.Parser.Wellformed

open PresqueYaml
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
                                                                                                  YamlNode.Scalar ""
                                                                                                  YamlNode.Scalar "F#" ] ] )
                                "user2", YamlNode.Mapping (Map [ "firstname", YamlNode.Scalar " J a n e  "
                                                                 "age", YamlNode.Scalar "666"
                                                                 "languages", YamlNode.Sequence [ YamlNode.Scalar "F# |> ❤️"
                                                                                                  YamlNode.Scalar "Python" ] ] )
                                "categories", YamlNode.Sequence [ YamlNode.Sequence [ YamlNode.Scalar "toto"
                                                                                      YamlNode.Scalar "titi"] ]
                                "fruits", YamlNode.Sequence [ YamlNode.Mapping (Map [ "cherry", YamlNode.Scalar "red" ])
                                                              YamlNode.Mapping (Map [ "banana", YamlNode.Scalar "yellow" ]) ] ])

    let yaml = "

#     user 1
user1:

  name: John Doe

  age:
  comment: this is a comment\\n😃
  languages: [ Python  , ,F# ]


   # this is a comment
user2:
  firstname: Toto
  lastname:   Doe

  age: 42
  languages:
    - F#
    -   Python

user2:
  firstname: ' J a n e  '

  age: 666
  languages: [ F# |> ❤️, Python ]

categories:
  - - toto
    - titi

fruits:
  - cherry: red
  - banana: yellow


"

    yaml
    |> Parser.read
    |> should equal expected




// // ####################################################################################################################

// [<Test>]
// let ``roundtrip``() =
//     let expected =
//         YamlNode.Mapping (Map [ "user1", YamlNode.Mapping (Map [ "name", YamlNode.Scalar "John Doe"
//                                                                  "age", YamlNode.None
//                                                                  "comment", YamlNode.Scalar "this is a comment\n😃"
//                                                                  "languages", YamlNode.Sequence [ YamlNode.Scalar "Python"
//                                                                                                   YamlNode.None
//                                                                                                   YamlNode.Scalar "F#" ] ] )
//                                 "user2", YamlNode.Mapping (Map [ "first name", YamlNode.Scalar "Toto"
//                                                                  "age", YamlNode.Scalar "666"
//                                                                  "languages", YamlNode.Sequence [ YamlNode.Scalar "F# |> ❤️"
//                                                                                                   YamlNode.Scalar "Python" ] ] )
//                                 "categories", YamlNode.Sequence [ YamlNode.Sequence [ YamlNode.Scalar "toto"
//                                                                                       YamlNode.Scalar "titi"] ]
//                                 "fruits", YamlNode.Sequence [ YamlNode.Mapping (Map [ "cherry", YamlNode.Scalar "red" ])
//                                                               YamlNode.Mapping (Map [ "banana", YamlNode.Scalar "yellow" ]) ] ])

//     let yaml = "categories:
//   -
//     -
//       toto
//     -
//       titi
// fruits:
//   -
//     cherry:
//       red
//   -
//     banana:
//       yellow
// user1:
//   age:
//   comment:
//     this is a comment
//     😃
//   languages:
//     -
//       Python
//     -
//       F#
//   name:
//     John Doe
// user2:
//   age:
//     666
//   'first name':
//     Toto
//   languages:
//     -
//       F# |> ❤️
//     -
//       Python"

//     yaml
//     |> Parser.read
//     |> should equal expected
