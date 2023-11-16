namespace MagnusOpera.PresqueYaml

[<RequireQualifiedAccess>]
type YamlNode =
    | None
    | Scalar of string
    | Sequence of YamlNode list
    | Mapping of Map<string, YamlNode>
