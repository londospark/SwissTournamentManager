module CreateTournament

open Fulma
open Fable.React.Helpers
open Fable.React

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
    | ChangedValue of Lens<State, string> * string
    | SubmitTournament

let update (msg: Msg) (state: State): State * Cmd<Msg> =
    match msg with
    | ChangedValue (Lens (_, put), value) -> put state value, Cmd.none
    | SubmitTournament -> state, Cmd.none

let get (Lens (get, _): Lens<'Outer, 'Inner>) (outer: 'Outer): 'Inner = get outer

let input (state: State) (dispatch: Msg -> unit) (lens: Lens<State, string>) (label: string) (placeholder: string): ReactElement =
    Field.div []
      [ Label.label [] [ str label ]
        Control.div [] [
            Input.text [
                Input.Placeholder placeholder
                Input.Value (get lens state)
                Input.OnChange (fun event -> dispatch (ChangedValue (lens, event.Value))) ] ] ]

let view (state: State) (dispatch: Msg -> unit) =
    let pageInput = input state dispatch

    [ Container.container [] [
        Heading.h2 [] [ str "Create Tournament" ]
        form []
          [ pageInput name "Name" "Advertising name."
            pageInput code "Code" "Tournament Code for entry." ] ] ]