namespace Players

[<CLIMutable>]
type Player = {
  name: string
}

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
