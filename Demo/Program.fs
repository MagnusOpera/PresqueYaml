open System.IO

let yamlString = File.ReadAllText("test-nested.yaml")
let parsedYaml = PresqueYaml.parse yamlString

// Affichage du résultat
printfn "Result : %A" parsedYaml
