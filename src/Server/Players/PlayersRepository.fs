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
    task {
      let! existingPlayer = getByName connectionString (PlayerName name)
      match existingPlayer with
      | Error ex -> return Error ex
      | Ok (Some player) -> return Ok player
      | Ok None ->
        let! affectedRows = insert connectionString name
        match affectedRows with
        | Error ex -> return Error ex
        | Ok _ ->
            let! player = getByName connectionString (PlayerName name)
            match player with
            | Ok (Some p) -> return Ok p
            | Ok None -> return Error (exn "Player added but not found on a second look.")
            | Error ex -> return Error ex
    }

