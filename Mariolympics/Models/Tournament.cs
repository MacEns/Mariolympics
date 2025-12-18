namespace Mariolympics.Models;

public class Tournament
{
    public int Id { get; set; }
    public DateTime Date { get; set; } = DateTime.Now;

    public List<Bracket> Brackets { get; set; } = [];

    /// <summary>
    /// Generates a complete tournament with brackets for all games
    /// </summary>
    /// <param name="players">List of players participating in the tournament</param>
    /// <returns>A new Tournament with generated brackets</returns>
    public static Tournament Generate(List<Player> players)
    {
        var tournament = new Tournament();

        // Create a bracket for each game
        foreach (var game in Game.All)
        {
            var bracket = GenerateBracket(players, game.Value);
            tournament.Brackets.Add(bracket);
        }

        return tournament;
    }

    /// <summary>
    /// Generates a single-elimination bracket for a specific game
    /// </summary>
    /// <param name="players">List of players participating</param>
    /// <param name="gameName">Name of the game for this bracket</param>
    /// <returns>A new Bracket with all rounds set up</returns>
    private static Bracket GenerateBracket(List<Player> players, string gameName)
    {
        var totalPlayers = players.Count;
        var nextPowerOfTwo = 1;
        while (nextPowerOfTwo < totalPlayers)
        {
            nextPowerOfTwo *= 2;
        }

        var byes = nextPowerOfTwo - totalPlayers;
        List<Player> firstRound = new List<Player>(players);

        // Add byes as null
        for (int i = 0; i < byes; i++)
        {
            firstRound.Add(null);
        }

        // Shuffle players
        Random rng = new Random();
        for (int i = firstRound.Count - 1; i > 0; i--)
        {
            int swapIndex = rng.Next(i + 1);
            (firstRound[i], firstRound[swapIndex]) = (firstRound[swapIndex], firstRound[i]);
        }

        // Create the bracket
        var bracket = new Bracket
        {
            Game = gameName,
            Rounds = new List<Round>()
        };

        int roundNumber = 1;
        List<(Player, Player)> matches = new List<(Player, Player)>();

        for (int i = 0; i < firstRound.Count; i += 2)
        {
            matches.Add((firstRound[i], firstRound[i + 1]));
        }

        var round1 = new Round
        {
            RoundNumber = roundNumber,
            Matches = matches.Select(m => new Match
            {
                Player1 = m.Item1,
                Player2 = m.Item2
            }).ToList()
        };

        // Set Round property on each match
        foreach (var match in round1.Matches)
        {
            match.Round = round1;
        }

        bracket.Rounds.Add(round1);

        // Generate subsequent rounds
        while (matches.Count > 1)
        {
            roundNumber++;
            List<(Player, Player)> nextRound = new List<(Player, Player)>();
            for (int i = 0; i < matches.Count; i += 2)
            {
                nextRound.Add((null, null)); // Placeholder for winners
            }

            matches = nextRound;
            var round = new Round
            {
                RoundNumber = roundNumber,
                Matches = matches.Select(m => new Match
                {
                    Player1 = m.Item1,
                    Player2 = m.Item2,
                }).ToList()
            };

            // Set Round property on each match
            foreach (var match in round.Matches)
            {
                match.Round = round;
            }

            bracket.Rounds.Add(round);
        }

        // Add third-place game
        if (roundNumber > 1)
        {
            var semifinalLosers = new List<Player>
            {
                null, // Placeholder for the first semifinal loser
                null  // Placeholder for the second semifinal loser
            };

            roundNumber++;
            var bronzeRound = new Round
            {
                RoundNumber = roundNumber,
                Matches = new List<Match>
                {
                    new Match
                    {
                        Player1 = semifinalLosers[0],
                        Player2 = semifinalLosers[1],
                    }
                }
            };

            // Set Round property on bronze medal match
            foreach (var match in bronzeRound.Matches)
            {
                match.Round = bronzeRound;
            }

            bracket.Rounds.Add(bronzeRound);
        }

        // Automatically advance players with byes to the next round
        AdvanceByePlayers(bracket);

        return bracket;
    }

    /// <summary>
    /// Advances players who have byes (null opponent) to the next round
    /// </summary>
    private static void AdvanceByePlayers(Bracket bracket)
    {
        if (bracket.Rounds == null || bracket.Rounds.Count < 2)
            return;

        var firstRound = bracket.Rounds[0];

        // Find all matches where one player has a bye
        foreach (var match in firstRound.Matches)
        {
            Player playerToAdvance = null;

            if (match.Player1 != null && match.Player2 == null)
            {
                // Player1 has a bye
                playerToAdvance = match.Player1;
            }
            else if (match.Player2 != null && match.Player1 == null)
            {
                // Player2 has a bye
                playerToAdvance = match.Player2;
            }

            if (playerToAdvance != null)
            {
                // Set the player as the winner (they advance automatically)
                match.SetWinner(playerToAdvance);

                // Advance them to the next round
                var matchIndex = firstRound.Matches.IndexOf(match);
                var nextMatchIndex = matchIndex / 2;
                var nextRound = bracket.Rounds[1];

                if (nextMatchIndex < nextRound.Matches.Count)
                {
                    var nextMatch = nextRound.Matches[nextMatchIndex];

                    // Place winner in the appropriate slot in the next round
                    if (nextMatch.Player1 == null)
                    {
                        nextMatch.Player1 = playerToAdvance;
                    }
                    else if (nextMatch.Player2 == null)
                    {
                        nextMatch.Player2 = playerToAdvance;
                    }
                }
            }
        }
    }
}
