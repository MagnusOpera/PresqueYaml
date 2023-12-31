# ✨ MagnusOpera.PresqueYaml

`PresqueYaml` is a Yaml parser and deserializer.

In French, "presque" means "almost". If you understand it right, `PresqueYaml` is not a fully compliant yaml parser. Don't worry, it does support most features!

<table><tr><td>If you want a strict 1.2 yaml parser, do not use this project: missing features will probably never be implemented as I consider them either misleading or niche cases.</td></tr></table>

`PresqueYaml` is written in F# and offers:
* Yaml deserialization to a representation model.
* Map representation model to an object model:
  * F# support: list<>, map<string,>, option<>, unit and record.
  * C# support: List<>, Dictionary<string,>, Nullable<> and class (via unique constructor).
  * YamlNode/YamlNodeValue<>: useful for structure validation (object model driven schema 😅).
* Support net7.0+ and NRT (Nullable Reference Types).
* Extensible, small and easily maintainable.

# 🧐 Key differences with Yaml (all versions)

<table><tr><td>It's probably not exhaustive ! Again, `PresqueYaml` does not pretend to support full Yaml specification. You have been warned if you heavily rely on this.</td></tr></table>

## Blocks
`PresqueYaml` only supports explicit and proper nested blocks as follow.

```
+--[block1]-------------x
|
|  +--[block2]----------x
|  |
|  x
|
|  +--[block3]----------x
|  |
|  |
|  x
|
x
```
A block can either be a mapping, sequence or scalar. In any cases, explicitness is mandatory - this is a key difference with Yaml specification (where implicit flow literal with dedent is allowed for eg).

## Scalars
* single and double quoted strings are handled exactly the same.
* scalar are either compact (and multilines with same indentation), folded or literal. Implicit flow scalar with dedent is not supported for eg.
* only escapes `\n`, `\r`, `\t`and `\s` are supported.
* representation model is always using `string`type - it does not attempt to identify types: that's subsequent analyzer or deserializer responsibility.

## Sequences
* compact sequences do not support quoted strings - only raw content.
* Sequence must be strictly indented respective to parent node.

## Mappings
* quoted string keys are not allowed - they are of string type (without any spaces) and ends with `:` character.
* mapping can have duplicated keys (last key wins).
* compact mappings (single line) are not supported.
* mappings can be nested on same line (think nested block) - it's not encouraged since it's a clear departure from Yaml.

## Comments
* a line is either empty, comment or content. Mixed content is not supported.

## Documents
* no support for multiple document in one yaml file.
* no schema support.
* no tag support.

# 🚀 Supported feature example
Anyway, most Yaml documents do not use advanced features.
Here is a document showing `PresqueYaml` capabilities:

```yaml

# scalars
scalar: this is a scalar
implicitFoldedScalarMultiline: this
                               is
                               a
                               scalar
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
# note: this is something not valid in Yaml spec - usage is discouraged
#       but it's valid per block definition in PresqueYaml
nestedMapping: item1: value1
               item2: value2

```

F# representation model:
```ocaml
YamlNode.Mapping (Map [ // scalars
                        "scalar", YamlNode.Scalar "this is a scalar"
                        "implicitFoldedScalarMultiline", YamlNode.Scalar "this is a scalar"
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
F# is preferred of course to explore the representation model. 
Namespace of API is `MagnusOpera.PresqueYaml`.

## Availability
[![Nuget](https://img.shields.io/nuget/v/MagnusOpera.PresqueYaml)](https://nuget.org/packages/MagnusOpera.PresqueYaml)

[![Build status](https://github.com/MagnusOpera/PresqueYaml/workflows/build/badge.svg)](https://github.com/MagnusOpera/PresqueYaml/actions?query=workflow%3Abuild)

## Requirements
`PresqueYaml` targets `net7.0` and more.

## Parser
In order to convert a yaml document to a representation model, use:
```ocaml
let yaml = "value: toto"
let node = YamlParser.Read yaml
// node is of type YamlNode
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
To convert a representation model to a .net object model, use:
```ocaml
let node = ... // representation model
let map = YamlSerializer.Deserialize<Map<string, string>>(node)
// map is of type Map<string, string>
```

### Special types
By default, `PresqueYaml` manages:
* Byte, Char, Int16, UInt16, Int32, UInt32, Int64, UInt64, String
* C# Array, List<_>, Dictionary<string, _>, Nullable<_>, class (must define primary constructor only)
* F# list<_>, Set<_>, Map<string, _>, option, unit, record
* YamlNode and YamlNodeValue<_>

`YamlNodeValue<_>` allows to discover if a value is provided:
* has not been provided (Undefined)
* has not been set (None)
* has been set (Value)

### Specific behaviors
By default, following types have a default value:

| Type                         | Default value                                |
|------------------------------|----------------------------------------------|
| reference types              | `null` if NRT enabled and type is nullable   |
| Collections (both F# and C#) | `empty`                                      |
| YamlNodeValue                | `YamlNodeValue.Undefined`                    |

This behavior can be changed by providing a `YamlDeserializationOptions` on `Deserialize`.

**⚠️ NOTE:** NRT support is provided at runtime (it's reflection based).
Depending on activation context, you may observe different behaviors:
* C# + Nullable enable: NRT are enforced and reference types can be nullable
* C# + Nullable disabled: NRT are disabled and reference types can't be nullable
* F#: NRT are not supported and reference types can't be nullable

### Customization
`YamlDeserializer` must be given a configuration. This configuration instructs:
* which mappers to use.
* how to deserialize `YamlNode.None` regarding collections.

There is a default configuration provided (`YamlDefaults.options`):
* Include all F# and C# types
* `YamlNode.None` forces empty collections.

| Property    | Default           | Description                                   |
|-------------|-------------------|-----------------------------------------------|
| Converters  | F# and C# mappers | List of converters to use for deserialization |
| NoneIsEmpty | true              | Convert YamlNode.None to empty collection     |
