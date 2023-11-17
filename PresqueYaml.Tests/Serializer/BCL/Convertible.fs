module MagnusOpera.PresqueYaml.Tests.Serializer.Convertible

open MagnusOpera.PresqueYaml
open NUnit.Framework
open FsUnit

// ####################################################################################################################

[<Test>]
let ``short conversion``() =
    let node = YamlNode.Scalar "42"

    YamlSerializer.Deserialize<System.Int16>(node)
    |> should equal 42s

// ####################################################################################################################

[<Test>]
let ``ushort conversion``() =
    let node = YamlNode.Scalar "42"

    YamlSerializer.Deserialize<System.UInt16>(node)
    |> should equal 42us

// ####################################################################################################################

[<Test>]
let ``int conversion``() =
    let node = YamlNode.Scalar "42"

    YamlSerializer.Deserialize<System.Int32>(node)
    |> should equal 42

// ####################################################################################################################

[<Test>]
let ``default int conversion``() =
    let node = YamlNode.None

    YamlSerializer.Deserialize<System.Int32>(node)
    |> should equal 0

// ####################################################################################################################

[<Test>]
let ``uint conversion``() =
    let node = YamlNode.Scalar "42"

    YamlSerializer.Deserialize<System.UInt32>(node)
    |> should equal 42u

// ####################################################################################################################

[<Test>]
let ``long conversion``() =
    let node = YamlNode.Scalar "42"

    YamlSerializer.Deserialize<System.Int64>(node)
    |> should equal 42L

// ####################################################################################################################

[<Test>]
let ``ulong conversion``() =
    let node = YamlNode.Scalar "42"

    YamlSerializer.Deserialize<System.UInt64>(node)
    |> should equal 42UL

// ####################################################################################################################

[<Test>]
let ``string conversion``() =
    let node = YamlNode.Scalar "42"

    YamlSerializer.Deserialize<string>(node)
    |> should equal "42"

// ####################################################################################################################

[<Test>]
let ``default string conversion``() =
    let node = YamlNode.None

    YamlSerializer.Deserialize<string>(node)
    |> should equal null

// ####################################################################################################################

[<Test>]
let ``char conversion``() =
    let node = YamlNode.Scalar "A"

    YamlSerializer.Deserialize<char>(node)
    |> should equal 'A'


// ####################################################################################################################

[<Test>]
let ``byte conversion``() =
    let node = YamlNode.Scalar "A"

    YamlSerializer.Deserialize<char>(node)
    |> should equal 65uy
