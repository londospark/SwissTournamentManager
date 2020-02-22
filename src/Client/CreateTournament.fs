module CreateTournament

open Fulma
open Fable.React.Helpers
open Fable.React

open Elmish

type PageState = {
    Name: string
    Code: string
}

let defaultState: PageState = { Name = ""; Code = "" }

type PageMsg =
    | ChangedName of string
    | ChangedCode of string
    | SubmitTournament

let update (msg: PageMsg) (state: PageState): PageState * Cmd<PageMsg> =
    match msg with
    | ChangedName name -> { state with Name = name }, Cmd.none
    | ChangedCode code -> { state with Code = code }, Cmd.none
    | SubmitTournament -> state, Cmd.none

let view (state: PageState) (dispatch: PageMsg -> unit) =
        [ Heading.h2 [] [ str "Create Tournament" ]
          form []
            [ Field.div []
                [ Label.label [] [ str "Name" ]
                  Control.div [] [
                      Input.text [
                          Input.Placeholder "Advertising name."
                          Input.Value state.Name
                          Input.OnChange (fun event -> dispatch (ChangedName event.Value)) ] ] ] ] ]