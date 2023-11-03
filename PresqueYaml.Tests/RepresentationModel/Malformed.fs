module PresqueYaml.Tests.RepresentationModel.Malformed

open PresqueYaml
open NUnit.Framework
open FsUnit

// ####################################################################################################################

[<Test>]
let ``mapping must be on same indentation``() =
    let yaml = "users: 42
 toto:
titi: tralala"

    (fun () -> yaml |> read |> ignore)
    |> should (throwWithMessage "Indentation error (line 2)") typeof<System.Exception>

// ####################################################################################################################

[<Test>]
let ``dedent mapping must restore parent indentation``() =
    let yaml = "users:
  toto:
    titi: tralala
   tutu: pouet"

    (fun () -> yaml |> read |> ignore)
    |> should (throwWithMessage "Indentation error (line 4)") typeof<System.Exception>

// ####################################################################################################################

[<Test>]
let ``sequence must be on same indentation``() =
    let yaml = "users:
 - toto
  - tralala"

    (fun () -> yaml |> read |> ignore)
    |> should (throwWithMessage "Indentation error (line 3)") typeof<System.Exception>

// ####################################################################################################################

[<Test>]
let ``mapping value override must be on same line``() =
    let yaml = "user:
  name:
   toto
   titi"

    (fun () -> yaml |> read |> ignore)
    |> should (throwWithMessage "Unexpected data type (line 3)") typeof<System.Exception>