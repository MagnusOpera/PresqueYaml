module MagnusOpera.PresqueYaml.Tests.Serializer.FSharpUnit

open MagnusOpera.PresqueYaml
open NUnit.Framework
open FsUnit


type Pouet = {
    Value: unit
}

// ####################################################################################################################

[<Test>]
let ``unit conversion``() =
    let node = YamlNode.Scalar "42"

    YamlSerializer.Deserialize<unit>(node, Defaults.options)
    |> should equal (())
