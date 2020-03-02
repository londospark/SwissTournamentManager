module CreateTournament

open Fulma
open Fable.React.Helpers
open Fable.React

open ViewHelpers
open Lenses

open Elmish

type State = {
    Name: string
    Code: string
}

let defaultState: State = { Name = ""; Code = "" }

let name = Lens ((fun state -> state.Name), (fun state name -> {state with Name = name}))
let code = Lens ((fun state -> state.Code), (fun state code -> {state with Code = code}))

type Msg =
    | ChangedValue of State
    | SubmitTournament

let update (msg: Msg) (state: State): State * Cmd<Msg> =
    match msg with
    | ChangedValue newState -> newState, Cmd.none
    | SubmitTournament -> state, Cmd.none

let view (state: State) (dispatch: Msg -> unit) =
    let pageInput = input state (ChangedValue >> dispatch)

    [ Container.container [] [
        Heading.h2 [] [ str "Create Tournament" ]
        form []
          [ pageInput name "Name" "Advertising name."
            pageInput code "Code" "Tournament Code for entry." ] ] ]