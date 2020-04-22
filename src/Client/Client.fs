module Client

open Elmish
open Elmish.React
open Fable.React
open Fable.React.Props
open Fetch.Types
open Thoth.Fetch


module R = Fable.React.Standard
module Mui = Fable.MaterialUI.Core

open Thoth.Json

open Feliz
open Feliz.Router

open Shared
open Fable.Core.JS
open Fable.MaterialUI

type PageState =
    | CreateTournamentState of CreateTournament.State
    | IndexState of Index.State
    | EnterTournamentState of EnterTournament.State
    | ManageTournamentState of ManageTournament.State
    | NotImplemented

type State =
    { UrlSegments: string list
      PageState: PageState }

type Msg =
    | UrlChanged of string list
    | CreateTournamentMsg of CreateTournament.Msg
    | IndexMsg of Index.Msg
    | EnterTournamentMsg of EnterTournament.Msg
    | ManageTournamentMsg of ManageTournament.Msg

let routeToPage (segments: string list): PageState * Cmd<Msg> =
    match segments with
    | [ ] -> IndexState Index.defaultState, Cmd.none
    | [ Route.Query [ "msg", flash ] ] -> IndexState { Index.defaultState with FlashMessage = flash }, Index.fetchTournamentsCommand |> Cmd.map IndexMsg
    | ["CreateTournament"] -> CreateTournamentState CreateTournament.defaultState, Cmd.none
    | ["Enter"; code] -> EnterTournamentState (EnterTournament.tournamentForCode code), Cmd.none
    | ["Manage"; code] -> ManageTournamentState (ManageTournament.tournamentForCode code), ManageTournament.loadContent |> Cmd.map ManageTournamentMsg
    | _ -> NotImplemented, Cmd.none

let init(): State * Cmd<Msg> =
    let segments = Router.currentUrl()
    let pageState, command = routeToPage segments
    let initialModel =
        { UrlSegments = segments
          PageState = pageState }

    initialModel, command

let update (msg: Msg) (currentModel: State): State * Cmd<Msg> =
    match currentModel.PageState, msg with
    | CreateTournamentState pageModel, CreateTournamentMsg pageMsg ->
        let (nextPageModel, cmd) = CreateTournament.update pageMsg pageModel
        let nextModel = { currentModel with PageState = CreateTournamentState nextPageModel }
        nextModel, cmd |> Cmd.map CreateTournamentMsg

    | EnterTournamentState pageModel, EnterTournamentMsg pageMsg ->
        let (nextPageModel, cmd) = EnterTournament.update pageMsg pageModel
        let nextModel = { currentModel with PageState = EnterTournamentState nextPageModel }
        nextModel, cmd |> Cmd.map EnterTournamentMsg

    | IndexState pageModel, IndexMsg pageMsg ->
        let (nextPageModel, cmd) = Index.update pageMsg pageModel
        let nextModel = { currentModel with PageState = IndexState nextPageModel }
        nextModel, cmd |> Cmd.map IndexMsg

    | ManageTournamentState pageModel, ManageTournamentMsg pageMsg ->
        let (nextPageModel, cmd) = ManageTournament.update pageMsg pageModel
        let nextModel = { currentModel with PageState = ManageTournamentState nextPageModel }
        nextModel, cmd |> Cmd.map ManageTournamentMsg

    | _, UrlChanged segments ->
        let pageState, command = routeToPage segments
        let nextModel = { UrlSegments = segments; PageState = pageState }
        nextModel, command

    | _ -> currentModel, Cmd.none

let mainLayout (body: ReactElement list): ReactElement =
    div []
        [ Mui.appBar [
            AppBarProp.Position AppBarPosition.Sticky]
              [  Mui.toolbar [] [
                  Mui.typography [ Variant TypographyVariant.H6 ] [ str "Swiss Tournament Manager"] ] ]
          yield! body ]


let view (state: State) (dispatch: Msg -> unit) =
    let currentPage =
        match state.PageState with
        | IndexState model ->
            Index.view model (IndexMsg >> dispatch)

        | CreateTournamentState model ->
            CreateTournament.view model (CreateTournamentMsg >> dispatch)

        | EnterTournamentState model ->
            EnterTournament.view model (EnterTournamentMsg >> dispatch)

        | ManageTournamentState model ->
            ManageTournament.view model (ManageTournamentMsg >> dispatch)

        | NotImplemented -> [ div [] [ str (sprintf "%A" state.UrlSegments) ] ]

    Router.router
        [ Router.onUrlChanged (UrlChanged >> dispatch)
          Router.application (mainLayout currentPage) ]

#if DEBUG
open Elmish.Debug
open Elmish.HMR
#endif

Program.mkProgram init update view
#if DEBUG
|> Program.withConsoleTrace
#endif
|> Program.withReactBatched "elmish-app"
#if DEBUG
|> Program.withDebugger
#endif
|> Program.run
