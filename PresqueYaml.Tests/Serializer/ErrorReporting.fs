module MagnusOpera.PresqueYaml.Tests.Serializer.ErrorReporting
open MagnusOpera.PresqueYaml
open NUnit.Framework
open FsUnit

type Titi = {
    String: string
    Value: int
}

type Toto = {
    Titi: Titi
    StringOption: string option
}

[<Test>]
let ``check error reporting``() =
    let node = YamlNode.Mapping (Map ["StringOption", YamlNode.Scalar "pouet"
                                      "Titi", YamlNode.Mapping (Map ["String", YamlNode.Scalar "Tralala"]) ])

    try
        node |> YamlSerializer.Deserialize<Toto> |> ignore
        Assert.Fail "Expecting deserialization failure"
    with
    | :? YamlSerializerException as ex ->
        ex.Message |> should equal "Error while deserializing Toto.Titi: parameter Value must be provided"
    | _ ->
        Assert.Fail "Unexpected deserialization failure"
