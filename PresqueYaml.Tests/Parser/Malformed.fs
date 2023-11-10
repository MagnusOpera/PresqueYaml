module PresqueYaml.Tests.Parser.Malformed

open PresqueYaml
open NUnit.Framework
open FsUnit

// ####################################################################################################################

[<Test>]
let ``mapping must be on same indentation``() =
    let yaml = "users: 42
 toto:
titi: tralala"

    (fun () -> yaml |> Parser.read |> ignore)
    |> should (throwWithMessage "Indentation error (line 2, column 2)") typeof<System.Exception>

// ####################################################################################################################

[<Test>]
let ``mapping must have same indentation 1/2``() =
    let yaml = "
  titi: tralala
 tutu: pouet"

    (fun () -> yaml |> Parser.read |> ignore)
    |> should (throwWithMessage "Indentation error (line 3, column 2)") typeof<System.Exception>

// ####################################################################################################################

[<Test>]
let ``mapping must have same indentation 2/2``() =
    let yaml = "
titi:
  toto:
    tutu: pouet
  tata: ddqddwqwd"

    (fun () -> yaml |> Parser.read |> ignore)
    |> should (throwWithMessage "Indentation error (line 3, column 3)") typeof<System.Exception>

// ####################################################################################################################

[<Test>]
let ``children sequence of mapping must have same indentation``() =
    let yaml = "users:
 - toto
  - tralala"

    (fun () -> yaml |> Parser.read |> ignore)
    |> should (throwWithMessage "Indentation error (line 3, column 3)") typeof<System.Exception>

// ####################################################################################################################

[<Test>]
let ``sequence parent aligned with parent mapping is not valid``() =
    let yaml = "name: John Doe
age: 42
languages:
- F#
- Python"

    (fun () -> yaml |> Parser.read |> ignore)
    |> should (throwWithMessage "Type mismatch (line 4, column 1)") typeof<System.Exception>

// ####################################################################################################################

[<Test>]
let ``mapping type mismatch is error``() =
    let yaml = "users: 42
- toto"

    (fun () -> yaml |> Parser.read |> ignore)
    |> should (throwWithMessage "Type mismatch (line 2, column 1)") typeof<System.Exception>
