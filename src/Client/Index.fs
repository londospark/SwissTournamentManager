module Index

open Shared

open Elmish
open MaterialViewHelpers

module R = Fable.React.Standard
module Mui = Fable.MaterialUI.Core

open Fable.React.Helpers
open Fable.React
open Fable.React.Props

open Thoth.Fetch
open Fable.MaterialUI

type State = {
    Tournaments: Tournament list
    FlashMessage: string
    }

let defaultState : State = { Tournaments = []; FlashMessage = "" }

type Msg =
    | FetchTournaments
    | TournamentListReceived of Tournament list

let fetchTournamentsCommand: Cmd<Msg> =
    Cmd.OfPromise.perform (fun () -> Fetch.fetchAs<Tournament list> "/api/tournaments") () TournamentListReceived

let update (msg: Msg) (currentModel: State): State * Cmd<Msg> =
    match msg with
    | FetchTournaments -> currentModel, fetchTournamentsCommand
    | TournamentListReceived tournaments ->
        let nextModel = { currentModel with Tournaments = tournaments }
        nextModel, Cmd.none

let qrcode tournament = img [ Src("/api/qrcode/" + tournament.Code) ]
let entryLink tournament = a [ Href ("/#Enter/" + tournament.Code) ] [ str ("/#Enter/" + tournament.Code) ]

let managementLink tournament = a [ Href ("/#Manage/" + tournament.Code) ] [ str ("/#Manage/" + tournament.Code) ]

let view (state: State) (dispatch: Msg -> unit) =
    [ card [
        MaterialViewHelpers.button "Fetch Tournaments" (fun _ -> dispatch FetchTournaments)
        MaterialViewHelpers.linkButton "Create Tournament" "/#CreateTournament"
        Mui.table [] [
        Mui.tableHead []
            [ yield Mui.tableRow []
                [   Mui.tableCell [] [ str "Name" ]
                    Mui.tableCell [] [ str "QR" ]
                    Mui.tableCell [] [ str "Entry Link" ]
                    Mui.tableCell [] [ str "Management Link" ] ] ]

        Mui.tableBody []
            [ for t in state.Tournaments ->
                Mui.tableRow []
                    [ Mui.tableCell [] [ str t.Name ]
                      Mui.tableCell [] [ qrcode t ]
                      Mui.tableCell [] [ entryLink t ]
                      Mui.tableCell [] [ managementLink t ] ] ] ] ] ]
