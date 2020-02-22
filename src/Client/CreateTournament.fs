module CreateTournament

open Fulma
open Fable.React.Helpers
open Fable.React

type PageState = {
    Name: string
    Code: string
}

let defaultState: PageState = { Name = ""; Code = "" }

type PageMsg =
    | ChangedName of string
    | ChangedCode of string
    | SubmitTournament

let view (state: PageState) (dispatch: PageMsg -> unit) =
        [ Heading.h2 [] [ str "Create Tournament" ]
          form []
            [ Field.div []
                [ Label.label [] [ str "Name" ]
                  Control.div [] [ Input.text [ Input.Placeholder "Advertising name." ] ] ] ] ]