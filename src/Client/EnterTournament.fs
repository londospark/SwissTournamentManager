module EnterTournament

open Fable.React

open MaterialViewHelpers
open Lenses
open Elmish

open Feliz
open Feliz.Router

open Shared
open Thoth.Fetch

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

let enterTournamentCommand (state: State) : Cmd<Msg> =
    let playerName = PlayerName state.PlayerName
    let code = state.TournamentCode
    let tournamentUri = sprintf "/api/tournaments/%s/entries" code
    Cmd.OfPromise.perform
                (fun (player: PlayerName) -> Fetch.post (tournamentUri, player, isCamelCase = true)) playerName
                (fun _ -> EnteredTournament)

let update (msg: Msg) (currentModel: State): State * Cmd<Msg> =
    match msg with
    | ChangedValue state -> state, Cmd.none
    | EnterTournament -> currentModel, (enterTournamentCommand currentModel)
    | EnteredTournament -> currentModel, Router.navigate ("", ["msg", "Tournament Entered!"])

let view (state: State) (dispatch: Msg -> unit) =
        let pageInput = input state (ChangedValue >> dispatch)

        [ card
              [ cardTitle (sprintf "Entering tournament: %s" state.TournamentCode)
                form []
                    [ pageInput player "Name" "Name or nickname"
                      button "Enter Tournament" (fun _ -> dispatch EnterTournament) ] ] ]