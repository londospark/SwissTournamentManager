module Model

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
    | ShowCreateTournamentPage
    | TournamentListReceived of Tournament list
    | UrlChanged of string list