namespace Tournaments

[<CLIMutable>]
type Tournament = {
  name: string
  code: string
}

module Mappers =
    let toShared (t: Tournament): Shared.Tournament =
        { Name = t.name; Code = t.code }

module Validation =
  let validate v =
    let validators = [
      fun u -> if isNull u.name then Some ("name", "Name shouldn't be empty") else None
    ]

    validators
    |> List.fold (fun acc e ->
      match e v with
      | Some (k,v) -> Map.add k v acc
      | None -> acc
    ) Map.empty
