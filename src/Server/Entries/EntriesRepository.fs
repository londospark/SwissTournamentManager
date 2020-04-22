namespace Entries

open Database
open Microsoft.Data.Sqlite
open System.Threading.Tasks
open FSharp.Control.Tasks.V2.ContextInsensitive

module Database =
  let getAll connectionString : Task<Result<Entry seq, exn>> =
    task {
      use connection = new SqliteConnection(connectionString)
      return! query connection "SELECT Tournament, Player FROM Entries" None
    }

  let getById connectionString id : Task<Result<Entry option, exn>> =
    task {
      use connection = new SqliteConnection(connectionString)
      return! querySingle connection "SELECT Tournament, Player FROM Entries WHERE Tournament=@Tournament" (Some <| dict ["id" => id])
    }

  let update connectionString v : Task<Result<int,exn>> =
    task {
      use connection = new SqliteConnection(connectionString)
      return! execute connection "UPDATE Entries SET Tournament = @Tournament, Player = @Player WHERE Tournament=@Tournament" v
    }

  let insert connectionString v : Task<Result<int,exn>> =
    task {
      use connection = new SqliteConnection(connectionString)
      return! execute connection "INSERT INTO Entries(Tournament, Player) VALUES (@Tournament, @Player)" v
    }

  let delete connectionString id : Task<Result<int,exn>> =
    task {
      use connection = new SqliteConnection(connectionString)
      return! execute connection "DELETE FROM Entries WHERE Tournament=@Tournament" (dict ["id" => id])
    }

