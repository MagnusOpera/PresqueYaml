# PresqueYaml

In French, "presque" means "almost". Hope you now understand the purpose and limitations of this library 😃.

PresqueYaml offers:
* Deserialization to a representation model (see it as an AST)
* Map this representation model to an object model
* Support for C# (List<>, Dictionary<,>, Nullable and POCO)
* Support for F# (list, map, option and records)

PresqueYaml features:
* compact form
* inline sequence with several limitations

Beware, PresqueYaml does not offer complete Yaml support and default to some non-standard behavior. Non-compliance highlights:
* scalar are always literal (\n between items) - never folded (concatenated with space). If you want spaces, use single line form.
* multi-lines scalar must be indented relative to start of first item.
* inline sequences do not support quoted strings. Use standard sequence if you need to.
* quoted strings are either single or double quoted strings. Only newline is escaped.
* compact form (both sequence and mapping) can be nested at will on same line.
* mapping can have duplicated keys (last key wins).
* empty document is a valid document.
* no support for multiple document in one Yaml file.

All in all, PresqueYaml does support this kind of document (⏎ to highlight spaces in document):
```
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
  'first name': 'J a n e '    ⏎
  'last name':   Doe  ⏎
  ⏎
  age: 42⏎
  languages:⏎
    - F#⏎
    -   Python⏎
⏎
user2:⏎
  'first name': Toto⏎
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

