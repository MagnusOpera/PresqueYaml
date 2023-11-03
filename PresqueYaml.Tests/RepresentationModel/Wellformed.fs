module PresqueYaml.Tests.RepresentationModel.Wellformed

open PresqueYaml
open NUnit.Framework
open FsUnit

// ####################################################################################################################

[<Test>]
let ``empty lines are ignored``() =
    let expected =
        YamlNode.Mapping (Map [ "user1", YamlNode.Mapping (Map [ "name", YamlNode.Scalar "John Doe"
                                                                 "age", YamlNode.None ] )
                                "user2", YamlNode.Mapping (Map [ "name", YamlNode.Scalar "Jane Doe"
                                                                 "age", YamlNode.Scalar "42" ] ) ])

    let yaml = "
    
    
user1:
             
  name: John Doe

  age:

                 
user2:
  name: Jane Doe
                
  age: 42
  


"

    yaml
    |> read
    |> should equal expected

