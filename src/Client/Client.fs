module Client

open Elmish
open Elmish.React
open Fable.React
open Fable.React.Props
open Fetch.Types
open Thoth.Fetch
open Fulma
open Thoth.Json

open Shared

// The model holds data that you want to keep track of while the application is running
// in this case, we are keeping track of a counter
// we mark it as optional, because initially it will not be available from the client
// the initial value will be requested from server
type State = { PageModel: ApplicationState option }

// The Msg type defines what events/actions can occur while the application is running
// the state of the application changes *only* in reaction to these events
type Msg =
    | InitialModelLoaded of ApplicationState
    | FetchTournaments
    | TournamentListReceived of Tournament list

let initialPage () = Fetch.fetchAs<ApplicationState> "/api/init"

let fetchTournamentsCommand : Cmd<Msg> =
    Cmd.OfPromise.perform
        (fun () -> Fetch.fetchAs<Tournament list> "/api/tournaments")
        ()
        TournamentListReceived

let tournaments (state: State) =
    match state.PageModel with
    | Some appState -> appState.Tournaments
    | _ -> []

// defines the initial state and initial command (= side-effect) of the application
let init () : State * Cmd<Msg> =
    let initialModel = { PageModel = None }
    let loadCountCmd =
        Cmd.OfPromise.perform initialPage () InitialModelLoaded
    initialModel, loadCountCmd

// The update function computes the next state of the application based on the current state and the incoming events/messages
// It can also run side-effects (encoded as commands) like calling the server via Http.
// these commands in turn, can dispatch messages to which the update function will react.
let update (msg : Msg) (currentModel : State) : State * Cmd<Msg> =
    match currentModel.PageModel, msg with
    | _, InitialModelLoaded initialCount->
        let nextModel = { PageModel = Some initialCount }
        nextModel, Cmd.none
    | _, FetchTournaments ->
        currentModel, fetchTournamentsCommand
    | Some state, TournamentListReceived tournaments ->
        let nextModel = { currentModel with PageModel = Some { state with Tournaments = tournaments }}
        nextModel, Cmd.none
    | _ -> currentModel, Cmd.none


let safeComponents =
    let components =
        span [ ]
           [ a [ Href "https://github.com/SAFE-Stack/SAFE-template" ]
               [ str "SAFE  "
                 str Version.template ]
             str ", "
             a [ Href "https://saturnframework.github.io" ] [ str "Saturn" ]
             str ", "
             a [ Href "http://fable.io" ] [ str "Fable" ]
             str ", "
             a [ Href "https://elmish.github.io" ] [ str "Elmish" ]
             str ", "
             a [ Href "https://fulma.github.io/Fulma" ] [ str "Fulma" ]

           ]

    span [ ]
        [ str "Version "
          strong [ ] [ str Version.app ]
          str " powered by: "
          components ]


let qrcode (tournament: Tournament) = img [ Src ( "/api/qrcode/" + tournament.Code) ]

let button txt onClick =
    Button.button
        [ Button.IsFullWidth
          Button.Color IsPrimary
          Button.OnClick onClick ]
        [ str txt ]

let view (model : State) (dispatch : Msg -> unit) =
    div []
        [ Navbar.navbar [ Navbar.Color IsPrimary ]
            [ Navbar.Item.div [ ]
                [ Heading.h2 [ ]
                    [ str "Swiss Tournament Manager" ] ] ]

          Container.container []
              [ button "Fetch Tournaments" (fun _ -> dispatch FetchTournaments) ]

          Container.container
            []
            [ table [] [
              yield tr [] [
                th [] [str "Name"]
                th [] [str "QR"] ]
              for t in tournaments model ->
                tr [] [ td [] [ str t.Name ]; td [] [ qrcode t ]]
            ]]

          Footer.footer [ ]
                [ Content.content [ Content.Modifiers [ Modifier.TextAlignment (Screen.All, TextAlignment.Centered) ] ]
                    [ safeComponents ] ] ]

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
