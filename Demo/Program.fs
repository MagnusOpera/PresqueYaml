open System.IO
open PresqueYaml.Model

let yamlString = File.ReadAllText("test-nested.yaml")
let parsedYaml = read yamlString

// Affichage du résultat
printfn "Result : %A" parsedYaml
