module MagnusOpera.PresqueYaml.Tests.Serializer.YamlNode

open MagnusOpera.PresqueYaml
open NUnit.Framework
open FsUnit

// ####################################################################################################################

[<Test>]
let ``yamlnode conversion``() =
    let node = YamlNode.Scalar "42"

    YamlSerializer.Deserialize<YamlNode>(node, Defaults.options)
    |> should equal node

// ####################################################################################################################
