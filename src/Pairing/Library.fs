namespace Pairing

module Domain =

    type Player = { Name: string; Score: int }
    type TournamentState = Player list

    type Pairing =
        | Game of Player * Player
        | Bye of Player

    let pairSimpleSwissRound (state: TournamentState): Pairing list =
        let rec generatePairing = function
        | player1::player2::remainingPlayers ->
            [
                yield Game (player1, player2)
                yield! generatePairing remainingPlayers
            ]
        | [player] -> [Bye (player)]
        | [] -> []

        generatePairing state
