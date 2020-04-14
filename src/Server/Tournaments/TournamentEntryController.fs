namespace Tournaments

open Saturn.Controller
open FSharp.Control.Tasks.ContextInsensitive
open Saturn
open Microsoft.AspNetCore.Http

module EntrySubController =

    type TournamentEntry = { Tournament: string; PlayerName: string; }

    let createAction (ctx: HttpContext) =
        task {
            let! entry = Controller.getModel<TournamentEntry>(ctx)
            return! Response.ok ctx ""
        }

    let resource (tournament: string) = controller {
        create createAction
        index (fun ctx -> (sprintf "Tournament index handler for %s" tournament ) |> Controller.text ctx)
    }