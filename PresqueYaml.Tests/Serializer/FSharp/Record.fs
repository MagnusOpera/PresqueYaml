module MagnusOpera.PresqueYaml.Tests.Serializer.Record
open MagnusOpera.PresqueYaml
open NUnit.Framework
open FsUnit



type Toto = {
    String: string
    StringOption: string option
    // StringVOption: string voption
    Int: int
    IntOption: int option
    List: string list
    // IntVOption: int voption
}


type DotnetPublish = {
    Configuration: string option
    Trim: bool option
    SingleFile: bool option
    Runtime: string option
}

// ####################################################################################################################

[<Test>]
let ``record conversion``() =
    let expected = {
        String = "tralala"
        StringOption = None
        // StringVOption = ValueSome "pouet"
        Int = 42
        IntOption = Some 666
        List = []
        // IntVOption = ValueSome -1
    }

    let node = YamlNode.Mapping (Map ["String", YamlNode.Scalar "tralala"
                                      "StringVOption", YamlNode.Scalar "pouet"
                                      "Int", YamlNode.Scalar "42"
                                      "IntOption", YamlNode.Scalar "666"
                                      "IntVOption", YamlNode.Scalar "-1" ])

    YamlSerializer.Deserialize<Toto>(node)
    |> should equal expected


// ####################################################################################################################

[<Test>]
let ``option record conversion``() =
    let expected = Some {
        String = "tralala"
        StringOption = None
        // StringVOption = ValueSome "pouet"
        Int = 42
        IntOption = Some 666
        List = []
        // IntVOption = ValueSome -1
    }

    let node = YamlNode.Mapping (Map ["String", YamlNode.Scalar "tralala"
                                      "StringVOption", YamlNode.Scalar "pouet"
                                      "Int", YamlNode.Scalar "42"
                                      "IntOption", YamlNode.Scalar "666"
                                      "IntVOption", YamlNode.Scalar "-1" ])

    YamlSerializer.Deserialize<Toto option>(node)
    |> should equal expected

// ####################################################################################################################

// ####################################################################################################################

[<Test>]
let ``bool conversion``() =
    let expected = {
        Configuration = Some "Release"
        Trim = Some true
        SingleFile = Some true
        Runtime = Some "linux-x64"
    }

    let node = YamlNode.Mapping (Map ["configuration", YamlNode.Scalar "Release"
                                      "trim", YamlNode.Scalar "true"
                                      "singleFile", YamlNode.Scalar "true"
                                      "runtime", YamlNode.Scalar "linux-x64" ])

    YamlSerializer.Deserialize<DotnetPublish>(node)
    |> should equal expected

// ####################################################################################################################


type ExtensionConfig = {
    Version: string option
    Container: YamlNodeValue<string>
    Parameters: Map<string, YamlNode>
}

type BuildConfig = {
    Storage: string option
    Extensions: Map<string, ExtensionConfig option>
}

[<Test>]
let ``empty mapping allowed``() =
    let expected = {
            Storage = None
            Extensions = Map [ "dotnet", Some { Version = None
                                                Container = YamlNodeValue.Value "mcr.microsoft.com/dotnet/sdk:7.0"
                                                Parameters = Map [ "configuration", YamlNode.Scalar "$(configuration)" ] }
                               "terraform", Some { Version = None
                                                   Container = YamlNodeValue.Undefined
                                                   Parameters = Map [ "workspace", YamlNode.Scalar "$(workspace)" ] }
                               "make", None
                               "npm", None
                               "docker", None ]
        }

    let yaml = "
extensions:
  dotnet:
    container: mcr.microsoft.com/dotnet/sdk:7.0
    parameters:
      configuration: $(configuration)
  terraform:
    parameters:
      workspace: $(workspace)
  make:
  npm:
  docker:
"

    let nodes = yaml |> YamlParser.Read
    printfn $"{nodes}"

    nodes
    |> YamlSerializer.Deserialize<BuildConfig>
    |> should equal expected
