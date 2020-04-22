module ManageTournament
open Shared

open Fable.React.Helpers
open Elmish
open Thoth.Fetch

module R = Fable.React.Standard
module Mui = Fable.MaterialUI.Core

open Fable.MaterialUI

type State = { Code: string; Players: PlayerName list }
type Msg =
    | PlayerListReceived of PlayerName list

let loadContent: Cmd<Msg> =
    Cmd.OfPromise.perform (fun () -> Fetch.fetchAs<PlayerName list> "/api/players") () PlayerListReceived

let defaultState = { Code = ""; Players = [] }
let tournamentForCode code = { defaultState with Code = code }

let update msg currentModel =
    match msg with
    | PlayerListReceived players -> { currentModel with Players = players}, Cmd.none

let view state dispatch =
    [ MaterialViewHelpers.card [
        Mui.cardHeader [ CardHeaderProp.Action (str <| sprintf "Managing Tournament %s" state.Code) ] [ ]
        Mui.cardContent [] [
            Mui.table [] [
                Mui.tableHead []
                    [ yield Mui.tableRow []
                        [   Mui.tableCell [] [ str "Player Name" ] ] ]

                Mui.tableBody []
                    [ for PlayerName p in state.Players ->
                        Mui.tableRow []
                            [ Mui.tableCell [] [ str p ] ] ] ] ] ] ]