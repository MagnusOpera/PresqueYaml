// module PresqueYaml.Tests.Model.Write

// open PresqueYaml.Model
// open NUnit.Framework
// open FsUnit

// // ####################################################################################################################

// [<Test>]
// let ``model to string``() =
//     let model =
//         YamlNode.Mapping (Map [ "user1", YamlNode.Mapping (Map [ "name", YamlNode.Scalar "John Doe"
//                                                                  "age", YamlNode.None
//                                                                  "comment", YamlNode.Scalar "this is a comment\n😃"
//                                                                  "languages", YamlNode.Sequence [ YamlNode.Scalar "Python"
//                                                                                                   YamlNode.Scalar "F#" ] ] )
//                                 "user2", YamlNode.Mapping (Map [ "first name", YamlNode.Scalar "Toto"
//                                                                  "age", YamlNode.Scalar "666"
//                                                                  "languages", YamlNode.Sequence [ YamlNode.Scalar "F# |> ❤️"
//                                                                                                   YamlNode.Scalar "Python" ] ] )
//                                 "categories", YamlNode.Sequence [ YamlNode.Sequence [ YamlNode.Scalar "toto"
//                                                                                       YamlNode.Scalar "titi"] ]
//                                 "fruits", YamlNode.Sequence [ YamlNode.Mapping (Map [ "cherry", YamlNode.Scalar "red" ])
//                                                               YamlNode.Mapping (Map [ "banana", YamlNode.Scalar "yellow" ]) ] ])

//     let expected = "user1:
//   name: John Doe
//   age:
//   comment: this is a comment\\n😃
//   languages:
//     - Python
//     - F#
// user2:
//   'first name': Toto
//   age: 666
//   languages:
//     - F# |> ❤️
//     - Python
// categories:
//   - - toto
//     - titi
// fruits:
//   - cherry: red
//   - banana: yellow"

//     let res =
//         model
//         |> write

//     printfn $"{res}"
    
//     res |> should equal expected
