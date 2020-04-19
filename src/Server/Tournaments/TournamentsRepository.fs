namespace Tournaments

open Database
open Microsoft.Data.Sqlite
open System.Threading.Tasks
open FSharp.Control.Tasks.ContextInsensitive

type Entry = { Tournament: string; Player: int64 }

module Database =
  let getAll connectionString : Task<Result<Tournament seq, exn>> =
    task {
      use connection = new SqliteConnection(connectionString)
      return! query connection "SELECT name, code FROM Tournaments" None
    }

  let getByCode connectionString code : Task<Result<Tournament option, exn>> =
    task {
      use connection = new SqliteConnection(connectionString)
      return! querySingle connection "SELECT name, code FROM Tournaments WHERE code=@code" (Some <| dict ["code" => code])
    }

  let update connectionString v : Task<Result<int,exn>> =
    task {
      use connection = new SqliteConnection(connectionString)
      return! execute connection "UPDATE Tournaments SET name = @name, code = @code WHERE name=@name" v
    }

  let insert connectionString v : Task<Result<int,exn>> =
    task {
      use connection = new SqliteConnection(connectionString)
      return! execute connection "INSERT INTO Tournaments(name, code) VALUES (@name, @code)" v
    }

  let delete connectionString id : Task<Result<int,exn>> =
    task {
      use connection = new SqliteConnection(connectionString)
      return! execute connection "DELETE FROM Tournaments WHERE name=@name" (dict ["id" => id])
    }

  let enter connectionString entry: Task<Result<int, exn>> =
    task {
        use connection = new SqliteConnection(connectionString)
        return! execute connection "INSERT INTO `Entries` (Tournament, Player) VALUES (@code, @player_id)" (dict ["code" => entry.Tournament; "player_id" => entry.Player])
    }

