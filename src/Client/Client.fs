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

// The model holds data that you want to keep track of while the application is running
// in this case, we are keeping track of a counter
// we mark it as optional, because initially it will not be available from the client
// the initial value will be requested from server
type State =
    { PageModel: ApplicationState option
      CurrentUrl: string list }

// The Msg type defines what events/actions can occur while the application is running
// the state of the application changes *only* in reaction to these events
type Msg =
    | InitialModelLoaded of ApplicationState
    | FetchTournaments
    | TournamentListReceived of Tournament list
    | UrlChanged of string list

let initialPage() = Fetch.fetchAs<ApplicationState> "/api/init"

let fetchTournamentsCommand: Cmd<Msg> =
    Cmd.OfPromise.perform (fun () -> Fetch.fetchAs<Tournament list> "/api/tournaments") () TournamentListReceived

let tournaments (state: State) =
    match state.PageModel with
    | Some appState -> appState.Tournaments
    | _ -> []

// defines the initial state and initial command (= side-effect) of the application
let init(): State * Cmd<Msg> =
    let initialModel =
        { PageModel = None
          CurrentUrl = Router.currentUrl() }

    let loadCountCmd = Cmd.OfPromise.perform initialPage () InitialModelLoaded
    initialModel, loadCountCmd

// The update function computes the next state of the application based on the current state and the incoming events/messages
// It can also run side-effects (encoded as commands) like calling the server via Http.
// these commands in turn, can dispatch messages to which the update function will react.
let update (msg: Msg) (currentModel: State): State * Cmd<Msg> =
    match currentModel.PageModel, msg with
    | _, InitialModelLoaded initialState ->
        let nextModel = { currentModel with PageModel = Some initialState }
        nextModel, Cmd.none
    | _, FetchTournaments -> currentModel, fetchTournamentsCommand
    | Some state, TournamentListReceived tournaments ->
        let nextModel = { currentModel with PageModel = Some { state with Tournaments = tournaments } }
        nextModel, Cmd.none
    | _, UrlChanged segments ->
        let nextModel = { currentModel with CurrentUrl = segments }
        nextModel, Cmd.none
    | _ -> currentModel, Cmd.none

let pageHeader =
    Navbar.navbar [ Navbar.Color IsInfo ]
        [ Navbar.Brand.div []
              [ Navbar.Link.a
                  [ Navbar.Link.IsArrowless
                    Navbar.Link.Props [ Href "#" ] ] [ str "Swiss Tournament Manager" ] ] ]

let qrcode (tournament: Tournament) = img [ Src("/api/qrcode/" + tournament.Code) ]

let button txt onClick =
    Button.button
        [ Button.IsFullWidth
          Button.Color IsPrimary
          Button.OnClick onClick ] [ str txt ]

let listPage (state: State) (dispatch: Msg -> unit) =
    div []
        [ pageHeader
          Container.container [] [ button "Fetch Tournaments" (fun _ -> dispatch FetchTournaments) ]

          Container.container []
              [ table []
                    [ yield tr []
                                [ th [] [ str "Name" ]
                                  th [] [ str "QR" ] ]
                      for t in tournaments state ->
                          tr []
                              [ td [] [ str t.Name ]
                                td [] [ qrcode t ] ] ] ] ]

let enter (code: string) (state: State) (dispatch: Msg -> unit) =
    div []
        [ pageHeader
          str (sprintf "Entering tournament %s" code) ]

let view (state: State) (dispatch: Msg -> unit) =
    let currentPage =
        match state.CurrentUrl with
        | [] -> listPage state dispatch
        | [ "Enter"; code ] -> enter code state dispatch
        | x -> div [] [ str (sprintf "%A" x) ]

    Router.router
        [ Router.onUrlChanged (UrlChanged >> dispatch)
          Router.application currentPage ]

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
