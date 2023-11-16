# ✨ MagnusOpera.PresqueYaml

In French, "presque" means "almost". If you understand it right, `PresqueYaml` is a yaml subset serialization library 😃.

<table><tr><td>If you want a strict 1.2 yaml parser, do not use this project: missing features will probably never be implemented as I consider them either misleading or niche cases.</td></tr></table>

`PresqueYaml` is written in F# and offers:
* Yaml deserialization to a representation model.
* Map representation model to an object model:
  * F# support: list, map, option, unit and record.
  * C# support: List<>, Dictionary<,>, Nullable<> and class (via unique constructor).
  * YamlNode/YamlNodeValue<>: this is useful for structure validation (object model driven schema).
* Support for net7.0+ only !
* Extensible
* It is small and easily maintainable

# 🧐 Key differences with Yaml 1.2
Few list of differences. It's probably not exhaustive !

<table><tr><td>Again, `PresqueYaml` does not offer complete yaml support, you have been warned if you heavily rely on this.</td></tr></table>

## Scalars
* single and double quoted strings are handled exactely the same.
* quoted string keys are not allowed - they are of string type and not scalar type.
* scalar are either compact (single line), folded or literal. You must choose one.
* only escapes `\n`, `\r`, `\t`and `\s` are supported.
* representation model is always using `string`type - it does not attempt to identify types: that's mapper responsibility.

## Sequences
* compact sequences do not support quoted strings - only raw content.

## Mappings
* mapping can have duplicated keys (last key wins).
* compact mappings (single line) are not supported.
* mappings can be nested.

## Comments
* a line is either empty, comment or content. Mixed content is not supported.

## Documents
* no support for multiple document in one yaml file.
* no schema support.
* no tag support.

# 🚀 Supported feature example
Anyway, most Yaml documents do not used advanced features.
Here is a document showing `PresqueYaml` capabilities:

```yaml

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

```

F# representation model:
```ocaml
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
```

# 📚 Api
`PresqueYaml` can be used both in F# and C#.
F# is preferred of course to explore the representation model (exposed in `MagnusOpera.PresqueYaml` namespace).

## Availability
[![Nuget](https://img.shields.io/nuget/v/MagnusOpera.PresqueYaml)](https://nuget.org/packages/MagnusOpera.PresqueYaml)
[![Build status](https://github.com/MagnusOpera/PresqueYaml/workflows/build/badge.svg)](https://github.com/MagnusOpera/PresqueYaml/actions?query=workflow%3Abuild)

## Requirements
`PresqueYaml` targets `netstandard2.1`. This practically means you can use .net 7.0 and more.

## Parser
In order to convert a yaml document to a representation model, use:
```ocaml
let yaml = "value: toto"
let node = MagnusOpera.PresqueYaml.YamlParser.Read yaml
// node is of type PresqueYaml.YamlNode
```

Representation model is:
```ocaml
[<RequireQualifiedAccess>]
type YamlNode =
    | None
    | Scalar of string
    | Sequence of YamlNode list
    | Mapping of Map<string, YamlNode>
```

## Deserialization
To convert a representation model to an .net object model, use:
```ocaml
let node = ... // representation model
let map = MagnusOpera.PresqueYaml.YamlSerializer.Deserialize<Map<string, string>>(node, Defaults.options)
// map is of type Map<string, string>
```

There is also a non-generic `Deserialize` method if you need more flexibility but for most cases, generic one shall be sufficient.

### Special types

By default, `PresqueYaml` manages:
* Byte, Char, Int16, UInt16, Int32, UInt32, Int64, UInt64, String
* C# Array, List<_>, Dictionary<string, _>, Nullable<_>, class (must define primary constructor only)
* F# list<_>, Set<_>, Map<string, _>, option, unit, record
* YamlNode and YamlNodeValue<_>

`YamlNodeValue<_>` allows to discover if a value:
* has not been provided (Undefined)
* has not been set (None)
* has been set (Value)

### Customization
`YamlDeserializer` must be given a configuration. This configuration instructs:
* which mappers to use.
* how to deserialize `YamlNode.None` regarding collections.

There is a default configuration provided (`PresqueYaml.Defaults.options`):
* Include all F# and C# types
* `YamlNode.None` forces empty collections.

| Property    | Default           | Description                                   |
|-------------|-------------------|-----------------------------------------------|
| Converters  | F# and C# mappers | List of converters to use for deserialization |
| NoneIsEmpty | true              | Convert YamlNode.None to empty collection     |
