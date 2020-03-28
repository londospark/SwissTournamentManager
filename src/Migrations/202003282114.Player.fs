namespace Migrations
open SimpleMigrations

[<Migration(202003282114L, "Create Players")>]
type CreatePlayers() =
  inherit Migration()

  override __.Up() =
    base.Execute(@"CREATE TABLE Players(
      name TEXT NOT NULL
    )")

  override __.Down() =
    base.Execute(@"DROP TABLE Players")
