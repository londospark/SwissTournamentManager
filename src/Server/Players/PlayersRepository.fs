namespace Players

open Database
open Microsoft.Data.Sqlite
open System.Threading.Tasks
open FSharp.Control.Tasks.ContextInsensitive

module Database =
  let getAll connectionString : Task<Result<Player seq, exn>> =
    task {
      use connection = new SqliteConnection(connectionString)
      return! query connection "SELECT name FROM Players" None
    }

  let getById connectionString id : Task<Result<Player option, exn>> =
    task {
      use connection = new SqliteConnection(connectionString)
      return! querySingle connection "SELECT name FROM Players WHERE name=@name" (Some <| dict ["id" => id])
    }

  let update connectionString v : Task<Result<int,exn>> =
    task {
      use connection = new SqliteConnection(connectionString)
      return! execute connection "UPDATE Players SET name = @name WHERE name=@name" v
    }

  let insert connectionString v : Task<Result<int,exn>> =
    task {
      use connection = new SqliteConnection(connectionString)
      return! execute connection "INSERT INTO Players(name) VALUES (@name)" v
    }

  let delete connectionString id : Task<Result<int,exn>> =
    task {
      use connection = new SqliteConnection(connectionString)
      return! execute connection "DELETE FROM Players WHERE name=@name" (dict ["id" => id])
    }

