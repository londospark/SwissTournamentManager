namespace Tournaments

open Saturn.Controller
open FSharp.Control.Tasks.ContextInsensitive
open Saturn
open Microsoft.AspNetCore.Http
open Shared
open Config

module EntrySubController =

    let createAction tournament (ctx: HttpContext) =
        task {
            let! playerName = Controller.getModel<PlayerName>(ctx)
            let cnf = Controller.getConfig ctx
            let! player = Players.Database.getOrCreateByName cnf.connectionString playerName
            match player with
            | Ok p ->
                // TODO(gareth): Players in the DB always have an id, players that we insert don't - maybe model this better?
                return! Response.ok ctx (sprintf "Player found with name %s and id %A" p.name p.id)
            | Error ex ->
                return raise ex
        }

    let resource (tournament: string) = controller {
        create (createAction tournament)
        index (fun ctx -> (sprintf "Tournament index handler for %s" tournament ) |> Controller.text ctx)
    }