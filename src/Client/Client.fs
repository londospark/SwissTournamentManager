module Client

open Elmish
open Elmish.React
open Fable.React
open Fable.React.Props
open Fetch.Types
open Thoth.Fetch
open Fulma
open Thoth.Json

open Feliz
open Feliz.Router

open Shared
open Fable.Core.JS

type PageState =
    | CreateTournamentState of CreateTournament.State
    | IndexState of Index.State
    | EnterTournamentState of EnterTournament.State
    | NotImplemented

type State =
    { UrlSegments: string list
      PageState: PageState }

type Msg =
    | UrlChanged of string list
    | CreateTournamentMsg of CreateTournament.Msg
    | IndexMsg of Index.Msg
    | EnterTournamentMsg of EnterTournament.Msg

let routeToPage (segments: string list): PageState =
    match segments with
    | [] -> IndexState Index.defaultState
    | ["CreateTournament"] -> CreateTournamentState CreateTournament.defaultState
    | ["Enter"; code] -> EnterTournamentState (EnterTournament.tournamentForCode code)
    | _ -> NotImplemented

let init(): State * Cmd<Msg> =
    let segments = Router.currentUrl()
    let initialModel =
        { UrlSegments = segments
          PageState = routeToPage segments }

    initialModel, Cmd.none

let update (msg: Msg) (currentModel: State): State * Cmd<Msg> =
    match currentModel.PageState, msg with
    | CreateTournamentState pageModel, CreateTournamentMsg pageMsg ->
        let (nextPageModel, cmd) = CreateTournament.update pageMsg pageModel
        let nextModel = { currentModel with PageState = CreateTournamentState nextPageModel }
        nextModel, cmd |> Cmd.map CreateTournamentMsg

    | IndexState pageModel, IndexMsg pageMsg ->
        let (nextPageModel, cmd) = Index.update pageMsg pageModel
        let nextModel = { currentModel with PageState = PageState.IndexState nextPageModel }
        nextModel, cmd |> Cmd.map IndexMsg

    | _, UrlChanged segments ->
        let nextModel = { UrlSegments = segments; PageState = routeToPage segments }
        nextModel, Cmd.none

    | _ -> currentModel, Cmd.none

let mainLayout (body: ReactElement list): ReactElement =
    div []
        [ Navbar.navbar [ Navbar.Color IsInfo ]
              [ Navbar.Brand.div []
                    [ Navbar.Link.a
                        [ Navbar.Link.IsArrowless
                          Navbar.Link.Props [ Href "#" ] ] [ str "Swiss Tournament Manager" ] ] ]
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
