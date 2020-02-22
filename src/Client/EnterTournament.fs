module EnterTournament

open Fulma
open Fable.React

type State = { TournamentCode: string }

type Msg =
    | NotImplemented

let defaultState: State = { TournamentCode = "" }
let tournamentForCode (code: string): State = { TournamentCode = code }

let view (state: State) (dispatch: Msg -> unit) =
        [ Container.container []
              [ Heading.h2 [ Heading.IsSubtitle ] [ str (sprintf "Entering tournament: %s" state.TournamentCode) ]
                form []
                    [ Field.div []
                          [ Label.label [] [ str "Name" ]
                            Control.div [] [ Input.text [ Input.Placeholder "For use in this tournament only." ] ] ] ] ] ]