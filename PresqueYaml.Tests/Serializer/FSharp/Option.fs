module MagnusOpera.PresqueYaml.Tests.Serializer.FSharpOption

open MagnusOpera.PresqueYaml
open MagnusOpera.PresqueYaml.Serializer
open NUnit.Framework
open FsUnit

// ####################################################################################################################

[<Test>]
let ``option some conversion``() =
    let node = YamlNode.Scalar "42"
    
    YamlSerializer.Deserialize<int option>(node, Defaults.options)
    |> should equal (Some 42)

// ####################################################################################################################

[<Test>]
let ``option none conversion``() =
    let node = YamlNode.None
    
    YamlSerializer.Deserialize<int option>(node, Defaults.options)
    |> should equal None
