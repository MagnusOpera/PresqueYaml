module PresqueYaml.Malformed.Tests

open PresqueYaml
open NUnit.Framework
open FsUnit

// ####################################################################################################################

[<Test>]
let ``wrong mapping indentation``() =
    let yaml = "users: 42
 toto:
titi: tralala"

    (fun () -> yaml |> parse |> ignore)
    |> should (throwWithMessage "Indentation error line 2") typeof<System.Exception>

// ####################################################################################################################

[<Test>]
let ``wrong mapping dedent indentation``() =
    let yaml = "users:
  toto:
    titi: tralala
   tutu: pouet"

    (fun () -> yaml |> parse |> ignore)
    |> should (throwWithMessage "Indentation error line 4") typeof<System.Exception>

// ####################################################################################################################

[<Test>]
let ``wrong sequence indentation``() =
    let yaml = "users:
 - toto
  - tralala"

    (fun () -> yaml |> parse |> ignore)
    |> should (throwWithMessage "Indentation error line 3") typeof<System.Exception>

// ####################################################################################################################

