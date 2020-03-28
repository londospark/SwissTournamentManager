namespace Tournaments

open Saturn.Controller
open FSharp.Control.Tasks.ContextInsensitive
open Saturn
open Microsoft.AspNetCore.Http

module EntrySubController =

    let createAction (ctx: HttpContext) =
        task {
            return! Response.badRequest ctx "Validation failed"
        }

    let resource (tournament: string) = controller {
        create createAction
        index (fun ctx -> (sprintf "Tournament index handler for %s" tournament ) |> Controller.text ctx)
    }