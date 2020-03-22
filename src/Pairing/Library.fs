namespace Pairing

module Domain =

    type Player = { Name: string; Score: int }
    type TournamentState = Player list

    type Pairing =
        | Game of Player * Player
        | Bye of Player

    let pairSimpleSwissRound (state: TournamentState): Pairing list =
        let gameCount = List.length state / 2
        let games = [ for i in 1..gameCount do Game ({ Name = ""; Score = 0 }, { Name = ""; Score = 0 })]
        if List.length state % 2 = 1 then (Bye {Name = ""; Score = 0}) :: games else games

