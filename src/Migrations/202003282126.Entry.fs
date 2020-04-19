namespace Migrations
open SimpleMigrations

[<Migration(202003282126L, "Create Entries")>]
type CreateEntries() =
  inherit Migration()

  override __.Up() =
    base.Execute(@"CREATE TABLE Entries(
      Tournament TEXT NOT NULL,
      Player INTEGER NOT NULL,
      PRIMARY KEY(Tournament, Player)

      FOREIGN KEY(Tournament) REFERENCES Tournaments(Code)
        ON DELETE CASCADE ON UPDATE NO ACTION,
      FOREIGN KEY(Player) REFERENCES Players(id)
        ON DELETE CASCADE ON UPDATE NO ACTION
    )")

  override __.Down() =
    base.Execute(@"DROP TABLE Entries")
