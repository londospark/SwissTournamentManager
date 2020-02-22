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

type PageModel =
    | CreateTournamentPage of CreateTournament.PageState
    | IndexPage of ApplicationState

type State =
    { CurrentUrl: string list
      Model: PageModel option }

// The Msg type defines what events/actions can occur while the application is running
// the state of the application changes *only* in reaction to these events
type Msg =
    | InitialModelLoaded of PageModel
    | FetchTournaments
    | ShowCreateTournamentPage
    | TournamentListReceived of Tournament list
    | UrlChanged of string list
    | CreateTournamentPage of CreateTournament.PageMsg

let initialPage(): Promise<PageModel> =
    promise {
        let! state = Fetch.fetchAs<ApplicationState> "/api/init"
        return IndexPage state
    }

let fetchTournamentsCommand: Cmd<Msg> =
    Cmd.OfPromise.perform (fun () -> Fetch.fetchAs<Tournament list> "/api/tournaments") () TournamentListReceived

// defines the initial state and initial command (= side-effect) of the application
let init(): State * Cmd<Msg> =
    let initialModel =
        { CurrentUrl = Router.currentUrl()
          Model = None }

    let initialiseModelCmd = Cmd.OfPromise.perform initialPage () InitialModelLoaded
    initialModel, initialiseModelCmd

// The update function computes the next state of the application based on the current state and the incoming events/messages
// It can also run side-effects (encoded as commands) like calling the server via Http.
// these commands in turn, can dispatch messages to which the update function will react.
let update (msg: Msg) (currentModel: State): State * Cmd<Msg> =
    match currentModel.Model, msg with // TODO(gareth): Fix horribleness... remove PageModel?
    | Some (PageModel.CreateTournamentPage pageModel), CreateTournamentPage pageMsg ->
        let (nextPageModel, cmd) = CreateTournament.update pageMsg pageModel
        let nextModel = { currentModel with Model = Some (PageModel.CreateTournamentPage nextPageModel) }
        nextModel, cmd |> Cmd.map CreateTournamentPage
    | _, ShowCreateTournamentPage ->
        { currentModel with Model = Some (PageModel.CreateTournamentPage CreateTournament.defaultState) }, Router.navigate("CreateTournament")
    | _, InitialModelLoaded initialState ->
        let nextModel = { currentModel with Model = Some initialState }
        nextModel, Cmd.none
    | _, FetchTournaments -> currentModel, fetchTournamentsCommand
    | Some (PageModel.IndexPage pageModel), TournamentListReceived tournaments ->
        let nextModel = { currentModel with Model = Some (IndexPage { pageModel with Tournaments = tournaments } )}
        nextModel, Cmd.none
    | _, UrlChanged segments ->
        let nextModel = { currentModel with CurrentUrl = segments }
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

let qrcode (tournament: Tournament) = img [ Src("/api/qrcode/" + tournament.Code) ]
let link (tournament: Tournament) = a [Href ("/#Enter/" + tournament.Code) ] [ str ("/#Enter/" + tournament.Code) ]

let button txt onClick =
    Button.button
        [ Button.IsFullWidth
          Button.Color IsPrimary
          Button.OnClick onClick ] [ str txt ]

let listPage (state: ApplicationState) (dispatch: Msg -> unit) =
        [ Container.container [] [
            Columns.columns [] [
                Column.column [] [button "Fetch Tournaments" (fun _ -> dispatch FetchTournaments) ]
                Column.column [] [button "Create Tournament" (fun _ -> dispatch ShowCreateTournamentPage) ] ] ]

          Container.container []
              [ Table.table []
                    [ thead []
                          [ yield tr []
                                      [ th [] [ str "Name" ]
                                        th [] [ str "QR" ]
                                        th [] [ str "Entry Link" ] ] ]
                      tbody []
                          [ for t in state.Tournaments ->
                              tr []
                                  [ td [] [ str t.Name ]
                                    td [] [ qrcode t ]
                                    td [] [ link t ] ] ] ] ] ]

let entryPage (code: string) (state: State) (dispatch: Msg -> unit) =
        [ Container.container []
              [ Heading.h2 [ Heading.IsSubtitle ] [ str (sprintf "Entering tournament: %s" code) ]
                form []
                    [ Field.div []
                          [ Label.label [] [ str "Name" ]
                            Control.div [] [ Input.text [ Input.Placeholder "For use in this tournament only." ] ] ] ] ] ]

let view (state: State) (dispatch: Msg -> unit) =
    let currentPage =
        match state.CurrentUrl, state.Model with
        | [], Some (PageModel.IndexPage model) -> listPage model dispatch
        | [ "CreateTournament" ], Some (PageModel.CreateTournamentPage model) -> CreateTournament.view model (CreateTournamentPage >> dispatch)
        | [ "Enter"; code ], _ -> entryPage code state dispatch
        | x -> [ div [] [ str (sprintf "%A" x) ] ]

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
