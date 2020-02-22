module CreateTournament

open Fulma
open Fable.React.Helpers
open Fable.React

open Elmish

type State = {
    Name: string
    Code: string
}

let defaultState: State = { Name = ""; Code = "" }

type Msg =
    | ChangedName of string
    | ChangedCode of string
    | SubmitTournament

let update (msg: Msg) (state: State): State * Cmd<Msg> =
    match msg with
    | ChangedName name -> { state with Name = name }, Cmd.none
    | ChangedCode code -> { state with Code = code }, Cmd.none
    | SubmitTournament -> state, Cmd.none

let view (state: State) (dispatch: Msg -> unit) =
        [ Heading.h2 [] [ str "Create Tournament" ]
          form []
            [ Field.div []
                [ Label.label [] [ str "Name" ]
                  Control.div [] [
                      Input.text [
                          Input.Placeholder "Advertising name."
                          Input.Value state.Name
                          Input.OnChange (fun event -> dispatch (ChangedName event.Value)) ] ] ] ] ]