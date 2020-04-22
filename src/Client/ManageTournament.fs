module ManageTournament
open Shared
open Fable.React.Standard
open Fable.React.Helpers
open Fable.MaterialUI
module R = Fable.React.Standard
module Mui = Fable.MaterialUI.Core


type State = { Code: string; Players: PlayerName list }
type Msg = Something

let defaultState = { Code = ""; Players = [] }
let tournamentForCode code = { defaultState with Code = code }
let update msg currentModel = ()
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