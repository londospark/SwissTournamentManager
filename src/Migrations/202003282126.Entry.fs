namespace Migrations
open SimpleMigrations

[<Migration(202003282126L, "Create Entries")>]
type CreateEntries() =
  inherit Migration()

  override __.Up() =
    base.Execute(@"CREATE TABLE Entries(
      Tournament INT NOT NULL,
      Player INT NOT NULL
    )")

  override __.Down() =
    base.Execute(@"DROP TABLE Entries")
