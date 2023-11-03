open System.IO

let yamlString = File.ReadAllText("test-nested.yaml")
let parsedYaml = PresqueYaml.read yamlString

// Affichage du résultat
printfn "Result : %A" parsedYaml
