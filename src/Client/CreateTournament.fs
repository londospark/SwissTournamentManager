module CreateTournament

open Model

open Fulma
open Fable.React.Helpers

let view (state: State) (dispatch: Msg -> unit) =
        [ Heading.h2 [] [ str "Create Tournament" ] ]