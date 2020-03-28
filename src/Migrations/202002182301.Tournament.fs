namespace Migrations
open SimpleMigrations

[<Migration(202002182301L, "Create Tournaments")>]
type CreateTournaments() =
  inherit Migration()

  override __.Up() =
    base.Execute(@"CREATE TABLE Tournaments(
      name TEXT NOT NULL,
      code TEXT NOT NULL PRIMARY KEY
    )")

  override __.Down() =
    base.Execute(@"DROP TABLE Tournaments")
