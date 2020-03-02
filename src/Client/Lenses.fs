module Lenses

type Lens<'Outer, 'Inner> = Lens of (('Outer -> 'Inner) * ('Outer -> 'Inner -> 'Outer))

let compose (Lens (getL, putL): Lens<'Outer, 'Middle>) (Lens (getR, putR): Lens<'Middle, 'Inner>): Lens<'Outer, 'Inner> =
    let put (outer: 'Outer) (inner: 'Inner): 'Outer =
        putL outer (putR (getL outer) inner)
    Lens (getL >> getR, put)

let (>->) l r = compose l r

let get (Lens (get, _): Lens<'Outer, 'Inner>) (outer: 'Outer): 'Inner = get outer
let set (Lens (_, set): Lens<'Outer, 'Inner>) (outer: 'Outer) (value: 'Inner): 'Outer = set outer value