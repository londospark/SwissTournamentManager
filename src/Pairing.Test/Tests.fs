module Tests

open Xunit
open FsCheck.Xunit

open Pairing.Domain

[<Property>]
let ``There are no byes when we have an even number of players`` (halfPlayers: int) =
    let playerCount: int = (abs halfPlayers) * 2

    let players : Player list =
        [ for i = 1 to playerCount do {Name = (sprintf "Player%d" i); Score = 0}]

    let tournamentState: TournamentState = players

    let pairings: Pairing list = pairSimpleSwissRound tournamentState
    Assert.All(pairings,
        fun pairing ->
            Assert.True(match pairing with Game _ -> true | _ -> false))


[<Property>]
let ``There is 1 bye when we have an odd number of players`` (halfPlayers: int) =
    let playerCount: int = (abs halfPlayers) * 2 + 1

    let players : Player list =
        [ for i in 1..playerCount do {Name = (sprintf "Player%d" i); Score = 0}]

    let tournamentState: TournamentState = players

    let pairings: Pairing list = pairSimpleSwissRound tournamentState
    let byes =
        pairings
        |> List.filter (function Bye _ -> true | _ -> false)
    Assert.Equal(1, List.length byes)

[<Property>]
let ``When we have an even number of players then the number of games is half the number of players`` (halfPlayers: int) =
    let playerCount: int = (abs halfPlayers) * 2

    let players : Player list =
        [ for i in 1..playerCount do {Name = (sprintf "Player%d" i); Score = 0}]

    let tournamentState: TournamentState = players

    let pairings: Pairing list = pairSimpleSwissRound tournamentState

    let expected = playerCount / 2
    let actual = List.length (pairings |> List.filter (function Game _ -> true | _ -> false))

    Assert.Equal(expected, actual)

[<Property>]
let ``When we have an odd number of players then the number of games is half the number of (players - 1)`` (halfPlayers: int) =
    let playerCount: int = (abs halfPlayers) * 2 + 1

    let players : Player list =
        [ for i in 1..playerCount do {Name = (sprintf "Player%d" i); Score = 0}]

    let tournamentState: TournamentState = players

    let pairings: Pairing list = pairSimpleSwissRound tournamentState

    let expected = (playerCount - 1) / 2
    let actual = List.length (pairings |> List.filter (function Game _ -> true | _ -> false))

    Assert.Equal(expected, actual)

[<Property>]
let ``All players are paired either in a game or have a bye``(players: int) =
    let playerCount = abs players
    let players : Player list =
        [ for i in 1..playerCount do {Name = (sprintf "Player%d" i); Score = 0}]

    let tournamentState: TournamentState = players
    let pairings: Pairing list = pairSimpleSwissRound tournamentState

    let playersPaired = [ for pairing in pairings do
                            match pairing with
                            | Game (p1, p2) ->
                                yield p1
                                yield p2

                            | Bye player -> yield player ]

    let sortedPaired = List.sort playersPaired
    let sortedPlayers = List.sort players

    Assert.Equal<Player list>(sortedPlayers, sortedPaired)