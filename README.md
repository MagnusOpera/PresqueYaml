# PresqueYaml

In French, "presque" means "almost". If you understand it right, `PresqueYaml` is a yaml subset serialization library 😃.

<table><tr><td>If you want a strict 1.2 yaml parser, do not use this project: missing features will probably never be implemented.</td></tr></table>

`PresqueYaml` is written in F# and offers:
* Yaml deserialization to a representation model (AST).
* Map representation model to an object model.
* C# support: List<>, Dictionary<,>, Nullable<> and POCO (via unique constructor).
* F# support: list, map, option, unit and record.

Again, `PresqueYaml` does not offer complete yaml support.

Here are some key differences:
* quoted strings are either single or double quoted strings - they are the same.
* quoted string keys are not allowed.
* mapping can have duplicated keys (last key wins).
* no support for multiple document in one yaml file.
* no schema support.

All in all, `PresqueYaml` does support this kind of document (⏎ to highlight spaces in document):
```yaml
⏎
#     user 1⏎
user1:⏎
      ⏎
  name: John Doe⏎
⏎
  age:⏎
  comment: this is a comment\\n😃    ⏎
  languages: [ Python  , ,F# ]  ⏎
    ⏎
⏎
   # this is a comment⏎
user2:⏎
  firstname: 'J a n e '    ⏎
  lastname:   Doe  ⏎
  ⏎
  age: 42⏎
  languages:⏎
    - F#⏎
    -   Python⏎
⏎
user2:⏎
  firstname: Toto⏎
⏎
  age: 666⏎
  languages: [ F# |> ❤️,⏎
             Python ]⏎
   ⏎
categories:⏎
  - - toto   ⏎
    - titi   ⏎
  ⏎
fruits:⏎
  - cherry: red⏎
  - banana: yellow⏎
 ⏎
⏎
```

Corresponding AST is:
```ocaml
YamlNode.Mapping (Map [ "user1", YamlNode.Mapping (Map [ "name", YamlNode.Scalar "John Doe"
                                                         "age", YamlNode.None
                                                         "comment", YamlNode.Scalar "this is a comment\n😃"
                                                         "languages", YamlNode.Sequence [ YamlNode.Scalar "Python"
                                                                                          YamlNode.None
                                                                                          YamlNode.Scalar "F#" ] ] )
                        "user2", YamlNode.Mapping (Map [ "firstname", YamlNode.Scalar "Toto"
                                                         "age", YamlNode.Scalar "666"
                                                         "languages", YamlNode.Sequence [ YamlNode.Scalar "F# |> ❤️"
                                                                                          YamlNode.Scalar "Python" ] ] )
                        "categories", YamlNode.Sequence [ YamlNode.Sequence [ YamlNode.Scalar "toto"
                                                                              YamlNode.Scalar "titi"] ]
                        "fruits", YamlNode.Sequence [ YamlNode.Mapping (Map [ "cherry", YamlNode.Scalar "red" ])
                                                      YamlNode.Mapping (Map [ "banana", YamlNode.Scalar "yellow" ]) ] ])
```
