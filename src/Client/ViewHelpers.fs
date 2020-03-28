module ViewHelpers

open Fulma
open Fable.React.Helpers
open Fable.React

open Lenses

let input<'State> (state: 'State) (onUpdate: 'State -> unit) (lens: Lens<'State, string>) (label: string) (placeholder: string): ReactElement =
    Field.div []
      [ Label.label [] [ str label ]
        Control.div [] [
            Input.text [
                Input.Placeholder placeholder
                Input.Value (get lens state)
                Input.OnChange (fun event -> onUpdate (set lens state event.Value)) ] ] ]

let button (text: string) (onClick: Browser.Types.MouseEvent -> unit) =
    Field.div []
        [ Control.div [] [
            Button.span
                [ Button.Color IsPrimary
                  Button.OnClick onClick]
                [ str text ] ] ]