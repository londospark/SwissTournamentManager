namespace Migrations
open SimpleMigrations

[<Migration(202003282126L, "Create Entries")>]
type CreateEntries() =
  inherit Migration()

  override __.Up() =
    base.Execute(@"CREATE TABLE Entries(
      Tournament TEXT NOT NULL,
      Player INT NOT NULL,
      PRIMARY KEY(Tournament, Player)

      FOREIGN KEY(Tournament) REFERENCES Touranments(Code)
        ON DELETE CASCADE ON UPDATE NO ACTION,
      FOREIGN KEY(Player) REFERENCES Players(rowid)
        ON DELETE CASCADE ON UPDATE NO ACTION
    )")

  override __.Down() =
    base.Execute(@"DROP TABLE Entries")
