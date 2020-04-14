namespace Tournaments

open Saturn.Controller
open FSharp.Control.Tasks.ContextInsensitive
open Saturn
open Microsoft.AspNetCore.Http
open Shared

module EntrySubController =

    let createAction tournament ctx =
        task {
            let! entry = Controller.getModel<PlayerName>(ctx)
            let response = {| Tournament = tournament; PlayerName = entry |}
            return! Response.ok ctx response
        }

    let resource (tournament: string) = controller {
        create (createAction tournament)
        index (fun ctx -> (sprintf "Tournament index handler for %s" tournament ) |> Controller.text ctx)
    }