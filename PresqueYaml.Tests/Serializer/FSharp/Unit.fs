module PresqueYaml.Tests.Serializer.FSharpUnit

open PresqueYaml
open PresqueYaml.Serializer
open NUnit.Framework
open FsUnit


type Pouet = {
    Value: unit
}

// ####################################################################################################################

[<Test>]
let ``unit conversion``() =
    let node = YamlNode.Scalar "42"
    
    YamlSerializer.Deserialize<unit>(node, PresqueYaml.Defaults.options)
    |> should equal (())
