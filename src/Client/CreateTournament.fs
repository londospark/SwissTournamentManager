module CreateTournament

open Fable.MaterialUI.Props

open Fable.React.Helpers
open Fable.React

module R = Fable.React.Standard
module Mui = Fable.MaterialUI.Core

open MaterialViewHelpers
open Lenses

open Thoth.Fetch

open Elmish
open Shared

open Feliz
open Feliz.Router
open Fable.MaterialUI
open Fable.Core
open Fable.React.Props

type State = Tournament

let defaultState: State =
    { Name = ""
      Code = "" }

let name = Lens((fun state -> state.Name), (fun state name -> { state with Name = name }))
let code = Lens((fun state -> state.Code), (fun state code -> { state with Code = code }))

type Msg =
    | ChangedValue of State
    | CreateTournament
    | CreatedTournament

let createTournamentCommand (state: State): Cmd<Msg> =
    match state.Code, state.Name with
    | "", "" -> Cmd.none
    | "", _ -> Cmd.none
    | _, "" -> Cmd.none
    | _, _ -> Cmd.OfPromise.perform
                (fun (tournament: Tournament) -> Fetch.post ("/api/tournaments", tournament, isCamelCase = true)) state
                (fun _ -> CreatedTournament)


let update (msg: Msg) (state: State): State * Cmd<Msg> =
    match msg with
    | ChangedValue newState -> newState, Cmd.none
    | CreateTournament -> state, createTournamentCommand state
    | CreatedTournament ->
        { Name = ""
          Code = "" }, Router.navigate ("", [ "msg", "Tournament Created!" ])

let view (state: State) (dispatch: Msg -> unit) =
    let pageInput = input state (ChangedValue >> dispatch)
    [ card
        [ cardTitle "Create Tournament"
          form []
                [ pageInput name "Name" "Advertising name."
                  pageInput code "Code" "Tournament Code for entry."
                  button "Create Tournament" (fun _ -> dispatch CreateTournament) ] ] ]
