namespace Entries

[<CLIMutable>]
type Entry = {
  Tournament: int
  Player: int
}

module Validation =
  let validate v =
    let validators = [
      fun u -> if isNull u.Tournament then Some ("Tournament", "Tournament shouldn't be empty") else None
    ]

    validators
    |> List.fold (fun acc e ->
      match e v with
      | Some (k,v) -> Map.add k v acc
      | None -> acc
    ) Map.empty
