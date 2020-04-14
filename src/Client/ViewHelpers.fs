module MaterialViewHelpers

open Fable.React.Props

module Mui = Fable.MaterialUI.Core
open Fable.MaterialUI.Props

open Lenses
open Fable.React.Helpers
open Fable.React
open Fable.MaterialUI.Core

let cardTitle (title: string): ReactElement = Mui.typography [ Variant TypographyVariant.H5 ] [ str title ]

let card (innerElements: ReactElement seq): ReactElement =
    Mui.card
        [ CardProp.Raised true
          Style
              [ CSSProp.MarginTop 24
                CSSProp.MarginLeft "auto"
                CSSProp.MarginRight "auto"
                CSSProp.Width 500
                CSSProp.AlignItems AlignItemsOptions.Center
                CSSProp.Padding 24 ] ]
        innerElements

let button (text: string) (onClick: Browser.Types.MouseEvent -> unit) =
    Mui.button
        [ MaterialProp.FullWidth true
          ButtonProp.Variant ButtonVariant.Contained
          MaterialProp.Color ComponentColor.Primary
          OnClick onClick ]
        [ str text ]

let linkButton (text: string) (href: string) =
    Mui.button [
        MaterialProp.FullWidth true
        ButtonProp.Variant ButtonVariant.Contained
        MaterialProp.Color ComponentColor.Primary
        HTMLAttr.Href href ]
        [ str text ]

let input<'State> (state: 'State) (onUpdate: 'State -> unit) (lens: Lens<'State, string>) (label: string) (placeholder: string): ReactElement =
    Mui.textField [
        Label (str label)
        HTMLAttr.Value (get lens state)
        HTMLAttr.Placeholder placeholder
        MaterialProp.Margin FormControlMargin.Normal
        HTMLAttr.Required true
        MaterialProp.FullWidth true
        DOMAttr.OnChange (fun event -> onUpdate (set lens state event.Value))
    ] []