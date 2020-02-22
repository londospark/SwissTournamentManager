module Index

open Shared

open Elmish
open Fulma
open Fable.React.Helpers
open Fable.React
open Fable.React.Props

open Thoth.Fetch

type State = { Tournaments: Tournament list }

let defaultState : State = { Tournaments = [] }

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


let button txt onClick =
    Button.button
        [ Button.IsFullWidth
          Button.Color IsPrimary
          Button.OnClick onClick ] [ str txt ]

let linkButton txt href =
    Button.a
        [ Button.IsFullWidth
          Button.Color IsPrimary
          Button.Props [ Href href ] ] [ str txt ]

let qrcode (tournament: Tournament) = img [ Src("/api/qrcode/" + tournament.Code) ]
let link (tournament: Tournament) = a [ Href ("/#Enter/" + tournament.Code) ] [ str ("/#Enter/" + tournament.Code) ]

let view (state: State) (dispatch: Msg -> unit) =
        [ Container.container [] [
            Columns.columns [] [
                Column.column [] [button "Fetch Tournaments" (fun _ -> dispatch FetchTournaments) ]
                Column.column [] [linkButton "Create Tournament" "/#CreateTournament" ] ] ]

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
