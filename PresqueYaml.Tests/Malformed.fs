module PresqueYaml.Malformed.Tests

open PresqueYaml
open NUnit.Framework
open FsUnit

// ####################################################################################################################

[<Test>]
let ``mapping must be on same indentation``() =
    let yaml = "users: 42
 toto:
titi: tralala"

    (fun () -> yaml |> parse |> ignore)
    |> should (throwWithMessage "Indentation error line 2") typeof<System.Exception>

// ####################################################################################################################

[<Test>]
let ``dedent mapping must restore parent indentation``() =
    let yaml = "users:
  toto:
    titi: tralala
   tutu: pouet"

    (fun () -> yaml |> parse |> ignore)
    |> should (throwWithMessage "Indentation error line 4") typeof<System.Exception>

// ####################################################################################################################

[<Test>]
let ``sequence must be on same indentation``() =
    let yaml = "users:
 - toto
  - tralala"

    (fun () -> yaml |> parse |> ignore)
    |> should (throwWithMessage "Indentation error line 3") typeof<System.Exception>

// ####################################################################################################################
