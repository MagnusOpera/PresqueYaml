module PresqueYaml.Tests.RepresentationModel.Malformed

open PresqueYaml.RepresentationModel
open NUnit.Framework
open FsUnit

// ####################################################################################################################

[<Test>]
let ``mapping must be on same indentation``() =
    let yaml = "users: 42
 toto:
titi: tralala"

    (fun () -> yaml |> read |> ignore)
    |> should (throwWithMessage "Indentation error (line 2, column 2)") typeof<System.Exception>

// ####################################################################################################################

[<Test>]
let ``dedent mapping must restore parent indentation``() =
    let yaml = "users:
  toto:
    titi: tralala
   tutu: pouet"

    (fun () -> yaml |> read |> ignore)
    |> should (throwWithMessage "Indentation error (line 4, column 4)") typeof<System.Exception>

// ####################################################################################################################

[<Test>]
let ``sequence must be on same indentation``() =
    let yaml = "users:
 - toto
  - tralala"

    (fun () -> yaml |> read |> ignore)
    |> should (throwWithMessage "Indentation error (line 3, column 3)") typeof<System.Exception>

// ####################################################################################################################

[<Test>]
let ``sequence parent aligned with parent mapping is not valid``() =
    let yaml = "name: John Doe
age: 42
languages:
- F#
- Python"

    (fun () -> yaml |> read |> ignore)
    |> should (throwWithMessage "Type mismatch (line 4, column 1)") typeof<System.Exception>

// ####################################################################################################################

[<Test>]
let ``mapping type mismatch is error``() =
    let yaml = "users: 42
- toto"

    (fun () -> yaml |> read |> ignore)
    |> should (throwWithMessage "Type mismatch (line 2, column 1)") typeof<System.Exception>
