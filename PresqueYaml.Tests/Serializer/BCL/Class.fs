module MagnusOpera.PresqueYaml.Tests.Serializer.Class

open MagnusOpera.PresqueYaml
open NUnit.Framework
open FsUnit



type Toto(String: string,
          StringOption: string option,
          Int: int,
          IntOption: int option) =
    member val String = String
    member val StringOption = StringOption
    member val Int = Int
    member val IntOption = IntOption


// ####################################################################################################################

[<Test>]
let ``class conversion``() =
    let expected = Toto("tralala", None, 42, Some 666)

    let node = YamlNode.Mapping (Map ["String", YamlNode.Scalar "tralala"
                                      "StringVOption", YamlNode.Scalar "pouet"
                                      "Int", YamlNode.Scalar "42"
                                      "IntOption", YamlNode.Scalar "666"
                                      "IntVOption", YamlNode.Scalar "-1" ])

    let result = YamlSerializer.Deserialize<Toto>(node, Defaults.options)
    result.String |> should equal expected.String
    result.StringOption |> should equal expected.StringOption
    result.Int |> should equal expected.Int
    result.IntOption |> should equal expected.IntOption


// ####################################################################################################################

[<Test>]
let ``option class conversion``() =
    let expected = Some (Toto("tralala", None, 42, Some 666))

    let node = YamlNode.Mapping (Map ["String", YamlNode.Scalar "tralala"
                                      "StringVOption", YamlNode.Scalar "pouet"
                                      "Int", YamlNode.Scalar "42"
                                      "IntOption", YamlNode.Scalar "666"
                                      "IntVOption", YamlNode.Scalar "-1" ])

    let result = YamlSerializer.Deserialize<Toto option>(node, Defaults.options)
    result |> Option.map (fun x -> x.String) |> should equal (expected |> Option.map (fun x -> x.String))
    result |> Option.map (fun x -> x.StringOption) |> should equal (expected |> Option.map (fun x -> x.StringOption))
    result |> Option.map (fun x -> x.Int) |> should equal (expected |> Option.map (fun x -> x.Int))
    result |> Option.map (fun x -> x.IntOption) |> should equal (expected |> Option.map (fun x -> x.IntOption))
