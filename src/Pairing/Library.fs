namespace Pairing

module Domain =

    type Player = { Name: string; Score: int }
    type TournamentState = Player list

    type Pairing =
        | Game of Player * Player
        | Bye of Player

    let pairSimpleSwissRound (state: TournamentState): Pairing list =
        let rec generatePairing = function
        | p1::p2::ps -> [ yield Game (p1, p2); yield! generatePairing ps]
        | [player] -> [Bye (player)]
        | [] -> []

        generatePairing state
