namespace Players

open Database
open Microsoft.Data.Sqlite
open System.Threading.Tasks
open FSharp.Control.Tasks.ContextInsensitive
open Shared

module Database =
  let getAll connectionString : Task<Result<Player seq, exn>> =
    task {
      use connection = new SqliteConnection(connectionString)
      return! query connection "SELECT id, name FROM Players" None
    }

  let getByName connectionString (PlayerName name) : Task<Result<Player option, exn>> =
    task {
      use connection = new SqliteConnection(connectionString)
      return! querySingle connection "SELECT id, name FROM Players WHERE name=@name" (Some <| dict ["name" => name])
    }

  let getById connectionString (id: int) : Task<Result<Player option, exn>> =
    task {
      use connection = new SqliteConnection(connectionString)
      return! querySingle connection "SELECT id, name FROM Players WHERE id=@id" (Some <| dict ["id" => id])
    }

  let update connectionString v : Task<Result<int,exn>> =
    task {
      use connection = new SqliteConnection(connectionString)
      return! execute connection "UPDATE Players SET name = @name WHERE id=@id" v
    }

  let insert connectionString v : Task<Result<int,exn>> =
    task {
      use connection = new SqliteConnection(connectionString)
      return! execute connection "INSERT INTO Players(name) VALUES (@name)" (dict ["name" => v])
    }

  let delete connectionString id : Task<Result<int,exn>> =
    task {
      use connection = new SqliteConnection(connectionString)
      return! execute connection "DELETE FROM Players WHERE name=@name" (dict ["name" => id])
    }

  let getOrCreateByName connectionString (PlayerName name) : Task<Result<Player, exn>> =
    // Helper functions
    let condExec f m = task {
        match m with
        | Error ex -> return Error ex
        | Ok (Some player) -> return Ok (Some player)
        | _ -> return! f()
    }

    let checkPlayerExists() = task {
        let! existingPlayer = getByName connectionString (PlayerName name)
        match existingPlayer with
        | Error ex -> return Error ex
        | Ok (Some player) -> return Ok (Some player)
        | Ok None -> return Ok None
    }

    let tryToInsertPlayer() = task {
        let! affectedRows = insert connectionString name
        match affectedRows with
        | Error ex -> return Error ex
        | Ok _ -> return Ok None
    }
    // ----------------

    // Logic main body
    task {
        let! myResult1 = checkPlayerExists()
        let! myResult2 = condExec tryToInsertPlayer myResult1
        let! myResult3 = condExec checkPlayerExists myResult2

        match myResult3 with
        | Error ex -> return Error ex
        | Ok (Some player) -> return Ok player
        | _ -> return Error (exn "Player added but not found on a second look.")
    }
