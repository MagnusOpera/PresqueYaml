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
    |> YamlParser.read
    |> should equal expected


[<Test>]
let allCapabilities() =
    let expected =
        YamlNode.Mapping (Map [ // scalars
                                "scalar", YamlNode.Scalar "this is a scalar"
                                "noneScalar", YamlNode.None
                                "scalarSingleQuoted", YamlNode.Scalar "  this is a single quoted \nscalar  "
                                "scalarDoubleQuoted", YamlNode.Scalar "  this is a double quoted \nscalar  "
                                "scalarFolded", YamlNode.Scalar "this\n  is\n    a\n      scalar"
                                "scalarLiteral", YamlNode.Scalar "this   is     a       scalar"

                                // sequences
                                "sequence", YamlNode.Sequence [ YamlNode.Scalar "item1"
                                                                YamlNode.Scalar "item2"
                                                                YamlNode.None
                                                                YamlNode.Scalar "item3" ]
                                "nestedSequence", YamlNode.Sequence [ YamlNode.Sequence [ YamlNode.Scalar "item11"
                                                                                          YamlNode.Scalar "item12" ]
                                                                      YamlNode.Sequence [ YamlNode.Scalar "item21"
                                                                                          YamlNode.Scalar "item22" ] ]
                                "flowSequence", YamlNode.Sequence [ YamlNode.Scalar "item1"
                                                                    YamlNode.Scalar "item2"
                                                                    YamlNode.Scalar "item3" ]
                                "mappingInSequence", YamlNode.Sequence [ YamlNode.Mapping (Map [ "item1", YamlNode.Scalar "value1"
                                                                                                 "item2", YamlNode.Scalar "value2" ])
                                                                         YamlNode.Mapping (Map [ "item3", YamlNode.Scalar "value3"
                                                                                                 "item4", YamlNode.None ]) ]

                                // mappings
                                "mapping", YamlNode.Mapping (Map [ "item1", YamlNode.Scalar "value1"
                                                                   "item2", YamlNode.Scalar "value2"
                                                                   "item3", YamlNode.None ])
                                "nestedMapping", YamlNode.Mapping (Map [ "item1", YamlNode.Scalar "value1"
                                                                         "item2", YamlNode.Scalar "value2" ]) ])

    let yaml = "

    # scalars
    scalar: this is a scalar
    noneScalar:
    scalarSingleQuoted: '  this is a single quoted \\nscalar  '
    scalarDoubleQuoted: \"  this is a double quoted \\nscalar  \"
    scalarFolded: |
      this
        is
          a
            scalar
    scalarLiteral: >
      this
        is
          a
            scalar

    # sequences
    sequence:
     - item1
     - item2
     -
     - item3
    nestedSequence:
      - - item11
        - item12
      - - item21
        - item22
    flowSequence: [   item1, item2, item3   ]
    mappingInSequence:
      - item1: value1
        item2: value2
      - item3: value3
        item4:

    # mappings
    mapping:
      item1: value1
      item2: value2
      item3:
    nestedMapping: item1: value1
                   item2: value2

    "

    yaml
    |> YamlParser.read
    |> should equal expected
