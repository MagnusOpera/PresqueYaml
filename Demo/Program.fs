open System.IO
open PresqueYaml

let yamlString = File.ReadAllText("test-nested.yaml")
let parsedYaml = Parser.read yamlString

// Affichage du résultat
printfn "Result : %A" parsedYaml
