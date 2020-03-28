module EnterTournament

open Fulma
open Fable.React

open ViewHelpers
open Lenses
open Elmish

type State = {
    TournamentCode: string
    PlayerName: string }

type Msg =
    | ChangedValue of State
    | NotImplemented

let defaultState: State = { TournamentCode = ""; PlayerName = "" }
let tournamentForCode (code: string): State = { defaultState with TournamentCode = code }

let player: Lens<State, string> =
    Lens ((fun state -> state.PlayerName ), (fun state code -> {state with PlayerName = code}))

let update (msg: Msg) (currentModel: State): State * Cmd<Msg> =
    match msg with
    | ChangedValue state -> state, Cmd.none
    | NotImplemented -> currentModel, Cmd.none

let view (state: State) (dispatch: Msg -> unit) =
        let pageInput = input state (ChangedValue >> dispatch)

        [ Container.container []
              [ Heading.h2 [ Heading.IsSubtitle ] [ str (sprintf "Entering tournament: %s" state.TournamentCode) ]
                form []
                    [ pageInput player "Name" "Name or nickname" ] ] ]