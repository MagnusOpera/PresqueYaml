# ✨ MagnusOpera.PresqueYaml
`PresqueYaml` is a Yaml parser and deserializer.

In French, "presque" means "almost". If you understand it right, `PresqueYaml` is not a fully compliant yaml parser. Don't worry, it does support most features!

`PresqueYaml` is written in F# and offers:
* Yaml deserialization to a representation model.
* Map representation model to an object model:
  * F# support: list<>, map<string,>, option<>, unit and record.
  * C# support: List<>, Dictionary<string,>, Nullable<> and class (via unique constructor).
  * YamlNode/YamlNodeValue<>: useful for structure validation (object model driven schema 😅).
* Support net7.0+ and NRT (Nullable Reference Types).
* Extensible, small and easily maintainable.
