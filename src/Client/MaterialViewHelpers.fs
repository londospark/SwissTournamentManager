module MaterialViewHelpers

open Fable.React.Props

module Mui = Fable.MaterialUI.Core
open Fable.MaterialUI.Props

open Lenses
open Fable.React.Helpers
open Fable.React

let button (text: string) (onClick: Browser.Types.MouseEvent -> unit) =
    Mui.button
        [ MaterialProp.FullWidth true
          ButtonProp.Variant ButtonVariant.Contained
          MaterialProp.Color ComponentColor.Primary
          OnClick onClick ]
        [ str text ]

let materialInput<'State> (state: 'State) (onUpdate: 'State -> unit) (lens: Lens<'State, string>) (label: string) (placeholder: string): ReactElement =
    Mui.textField [
        Label (str label)
        HTMLAttr.Value (get lens state)
        HTMLAttr.Placeholder placeholder
        DOMAttr.OnChange (fun event -> onUpdate (set lens state event.Value))
    ] []