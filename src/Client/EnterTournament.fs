module EnterTournament

open Fable.React

open MaterialViewHelpers
open Lenses
open Elmish

open Feliz
open Feliz.Router

type State = {
    TournamentCode: string
    PlayerName: string }

type Msg =
    | ChangedValue of State
    | EnterTournament
    | EnteredTournament

let defaultState: State = { TournamentCode = ""; PlayerName = "" }
let tournamentForCode (code: string): State = { defaultState with TournamentCode = code }

let player: Lens<State, string> =
    Lens ((fun state -> state.PlayerName ), (fun state code -> {state with PlayerName = code}))

let enterTournamentCommand : Cmd<Msg> =
    Cmd.ofMsg EnteredTournament
    // Cmd.OfPromise.perform ...

let update (msg: Msg) (currentModel: State): State * Cmd<Msg> =
    match msg with
    | ChangedValue state -> state, Cmd.none
    | EnterTournament -> currentModel, enterTournamentCommand
    | EnteredTournament -> currentModel, Router.navigate ("", ["msg", "Tournament Entered!"])

let view (state: State) (dispatch: Msg -> unit) =
        let pageInput = materialInput state (ChangedValue >> dispatch)

        [ card
              [ cardTitle (sprintf "Entering tournament: %s" state.TournamentCode)
                form []
                    [ pageInput player "Name" "Name or nickname"
                      button "Enter Tournament" (fun _ -> dispatch EnterTournament) ] ] ]