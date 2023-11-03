open System.IO
open PresqueYaml.RepresentationModel

let yamlString = File.ReadAllText("test-nested.yaml")
let parsedYaml = read yamlString

// Affichage du résultat
printfn "Result : %A" parsedYaml
