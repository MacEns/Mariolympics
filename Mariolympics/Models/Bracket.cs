namespace Mariolympics.Models;

public class Bracket
{
    public int Id { get; set; }

    public Tournament Tournament { get; set; }
    public int TournamentId { get; set; }

    public string Game { get; set; }

    public List<Round> Rounds { get; set; } = new List<Round>();

    // Flag to determine if this bracket includes a bronze medal (third place) match
    public bool HasBronzeMedalMatch { get; set; } = true;

    // The bronze medal match (if enabled)
    public Match BronzeMedalMatch { get; set; }

    // Store the original bracket state for proper reset functionality
    private Dictionary<(int roundNumber, int matchIndex, int playerSlot), Player> _originalByePlacements = new Dictionary<(int, int, int), Player>();

    public void GenerateBracket(List<Player> players)
    {
        // Clear existing rounds
        Rounds = new List<Round>();

        if (players == null || !players.Any())
            return;

        // Calculate the number of first round matches needed
        var totalSlots = 1;
        while (totalSlots < players.Count)
        {
            totalSlots *= 2;
        }

        var firstRoundSlots = totalSlots;
        var byeCount = totalSlots - players.Count;
        var firstRoundMatches = (totalSlots - byeCount) / 2;

        var rounds = new List<Round>();
        var round1 = new Round { RoundNumber = 1 };
        round1.Matches = new List<Match>();

        // Create matches for players who need to play in round 1
        // Lower seeded players (end of list) play first, higher seeded get byes
        var playingPlayers = players.Take(players.Count - byeCount).ToList();

        for (int i = 0; i < playingPlayers.Count; i += 2)
        {
            var match = new Match { Round = round1 };
            match.AddPlayer(playingPlayers[i]);
            if (i + 1 < playingPlayers.Count)
            {
                match.AddPlayer(playingPlayers[i + 1]);
            }
            round1.Matches.Add(match);
        }

        rounds.Add(round1);

        // Create subsequent rounds
        var lastRound = rounds.Last();
        var totalPlayersInNextRound = firstRoundMatches + byeCount; // Winners from R1 + bye players

        while (totalPlayersInNextRound > 1)
        {
            var nextRound = new Round { RoundNumber = rounds.Count + 1 };
            nextRound.Matches = new List<Match>();

            var matchesInNextRound = totalPlayersInNextRound / 2;
            for (int i = 0; i < matchesInNextRound; i++)
            {
                var match = new Match { Round = nextRound };
                nextRound.Matches.Add(match);
            }

            rounds.Add(nextRound);
            totalPlayersInNextRound = matchesInNextRound;
        }

        // Place bye players directly into round 2
        if (rounds.Count > 1 && byeCount > 0)
        {
            var round2 = rounds[1];
            var byePlayers = players.Skip(players.Count - byeCount).ToList(); // Highest seeded players

            // Place bye players in the later matches of round 2
            for (int i = 0; i < byePlayers.Count; i++)
            {
                var matchIndex = round2.Matches.Count - 1 - (i / 2);
                if (matchIndex >= 0 && matchIndex < round2.Matches.Count)
                {
                    if (i % 2 == 0)
                    {
                        round2.Matches[matchIndex].Player2 = byePlayers[i];
                    }
                    else
                    {
                        round2.Matches[matchIndex].Player1 = byePlayers[i];
                    }
                }
            }
        }

        Rounds = rounds;

        // Create bronze medal match if enabled and we have enough players
        if (HasBronzeMedalMatch && players.Count >= 4)
        {
            BronzeMedalMatch = new Match();
            // The bronze medal match will be populated when the semi-finals are completed
        }
        else
        {
            BronzeMedalMatch = null;
        }

        // Store the original bracket state for reset functionality
        _originalByePlacements.Clear();
        for (int roundIndex = 0; roundIndex < Rounds.Count; roundIndex++)
        {
            var round = Rounds[roundIndex];
            for (int matchIndex = 0; matchIndex < round.Matches.Count; matchIndex++)
            {
                var match = round.Matches[matchIndex];

                // Store player positions for rounds after the first (these include bye players)
                if (round.RoundNumber > 1)
                {
                    if (match.Player1 != null)
                    {
                        _originalByePlacements[(round.RoundNumber, matchIndex, 1)] = match.Player1;
                    }
                    if (match.Player2 != null)
                    {
                        _originalByePlacements[(round.RoundNumber, matchIndex, 2)] = match.Player2;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Gets the current champion of the bracket (winner of the final match)
    /// </summary>
    public Player GetChampion()
    {
        var finalRound = Rounds.LastOrDefault();
        return finalRound?.Matches.FirstOrDefault()?.Winner;
    }

    /// <summary>
    /// Gets the bronze medal winner (third place)
    /// </summary>
    public Player GetBronzeMedalWinner()
    {
        return BronzeMedalMatch?.Winner;
    }

    /// <summary>
    /// Gets the total number of matches in the bracket
    /// </summary>
    public int GetTotalMatches()
    {
        var totalMatches = Rounds.SelectMany(r => r.Matches).Count();
        if (BronzeMedalMatch != null)
        {
            totalMatches++;
        }
        return totalMatches;
    }

    /// <summary>
    /// Gets the number of completed matches in the bracket
    /// </summary>
    public int GetCompletedMatches()
    {
        var completedMatches = Rounds.SelectMany(r => r.Matches).Count(m => m.Winner != null);
        if (BronzeMedalMatch?.Winner != null)
        {
            completedMatches++;
        }
        return completedMatches;
    }

    /// <summary>
    /// Checks if the tournament is complete (all matches have winners)
    /// </summary>
    public bool IsComplete()
    {
        return GetChampion() != null;
    }

    /// <summary>
    /// Gets the bracket completion percentage
    /// </summary>
    public double GetCompletionPercentage()
    {
        var total = GetTotalMatches();
        if (total == 0)
            return 0;
        return (double)GetCompletedMatches() / total * 100;
    }

    /// <summary>
    /// Sets the winner of a match and automatically advances them to the next round
    /// </summary>
    /// <param name="match">The match to set the winner for</param>
    /// <param name="winner">The winning player</param>
    /// <returns>True if the winner was set successfully, false otherwise</returns>
    public bool SetWinner(Match match, Player winner)
    {
        if (winner == null || match == null)
            return false;

        // Set the winner for the current match
        match.SetWinner(winner);

        // Handle bronze medal match
        if (match == BronzeMedalMatch)
        {
            // Bronze medal match completed, no further advancement needed
            return true;
        }

        // Find the next round
        var roundNumber = match.Round.RoundNumber;
        var nextRound = Rounds.FirstOrDefault(r => r.RoundNumber == roundNumber + 1);
        if (nextRound == null)
        {
            // This was the final match - check if we need to set up bronze medal match
            if (BronzeMedalMatch != null && BronzeMedalMatch.Player1 == null && BronzeMedalMatch.Player2 == null)
            {
                // Final match just completed, but bronze medal match was already set up from semi-finals
                // This shouldn't happen in normal flow, but just in case
            }
            return true;
        }

        // Calculate which match in the next round this winner should advance to
        var matchIndex = match.Round.Matches.IndexOf(match);
        var nextMatchIndex = matchIndex / 2;

        var nextRoundMatch = nextRound.Matches.ElementAtOrDefault(nextMatchIndex);
        if (nextRoundMatch == null)
        {
            return false;
        }

        // Place winner in the appropriate slot in the next round
        if (nextRoundMatch.Player1 == null)
        {
            nextRoundMatch.Player1 = winner;
        }
        else if (nextRoundMatch.Player2 == null)
        {
            nextRoundMatch.Player2 = winner;
        }
        else
        {
            // Both slots are already filled - this shouldn't happen in a valid bracket
            return false;
        }

        // Check if this was a semi-final match and both semi-finals are now complete
        if (BronzeMedalMatch != null && nextRound.RoundNumber == Rounds.Count)
        {
            // Check if both semi-final matches are now complete
            var semiFinalRound = Rounds[Rounds.Count - 2];
            var allSemiFinalsComplete = semiFinalRound.Matches.All(m => m.Winner != null);

            if (allSemiFinalsComplete && BronzeMedalMatch.Player1 == null && BronzeMedalMatch.Player2 == null)
            {
                // Both semi-finals are complete, set up the bronze medal match with the losers
                SetupBronzeMedalMatch();
            }
        }

        return true;
    }

    /// <summary>
    /// Gets all matches that are ready to be played (both players assigned, no winner yet)
    /// </summary>
    public List<Match> GetPlayableMatches()
    {
        var playableMatches = Rounds.SelectMany(r => r.Matches)
            .Where(m => m.Player1 != null && m.Player2 != null && m.Winner == null)
            .ToList();

        // Add bronze medal match if it's playable and all prerequisites are met
        if (IsBronzeMedalMatchReady())
        {
            playableMatches.Add(BronzeMedalMatch);
        }

        return playableMatches;
    }

    /// <summary>
    /// Checks if the bronze medal match is ready to be played
    /// </summary>
    private bool IsBronzeMedalMatchReady()
    {
        return BronzeMedalMatch != null &&
               BronzeMedalMatch.Player1 != null &&
               BronzeMedalMatch.Player2 != null &&
               BronzeMedalMatch.Winner == null &&
               AreSemiFinalsComplete();
    }

    /// <summary>
    /// Checks if all semi-final matches are complete
    /// </summary>
    private bool AreSemiFinalsComplete()
    {
        if (Rounds.Count < 2)
            return false;

        var semiFinalRound = Rounds[Rounds.Count - 2];
        return semiFinalRound.Matches.All(m => m.Winner != null);
    }

    /// <summary>
    /// Gets the next match that needs to be played in the tournament
    /// </summary>
    public Match GetNextMatch()
    {
        return GetPlayableMatches().FirstOrDefault();
    }    /// <summary>
         /// Resets the bracket by clearing all match winners while preserving original bye players
         /// </summary>
    public void Reset()
    {
        // Clear all winners and reset to original bracket state
        foreach (var round in Rounds)
        {
            foreach (var match in round.Matches)
            {
                match.Winner = null;

                // For rounds after the first, restore to original state
                if (round.RoundNumber > 1)
                {
                    match.Player1 = null;
                    match.Player2 = null;
                }
            }
        }

        // Reset bronze medal match
        if (BronzeMedalMatch != null)
        {
            BronzeMedalMatch.Player1 = null;
            BronzeMedalMatch.Player2 = null;
            BronzeMedalMatch.Winner = null;
        }

        // Restore original bye player placements
        foreach (var placement in _originalByePlacements)
        {
            var (roundNumber, matchIndex, playerSlot) = placement.Key;
            var player = placement.Value;

            var round = Rounds.FirstOrDefault(r => r.RoundNumber == roundNumber);
            if (round != null && matchIndex < round.Matches.Count)
            {
                var match = round.Matches[matchIndex];
                if (playerSlot == 1)
                {
                    match.Player1 = player;
                }
                else if (playerSlot == 2)
                {
                    match.Player2 = player;
                }
            }
        }
    }

    /// <summary>
    /// Sets up the bronze medal match with the losers of the semi-final matches
    /// </summary>
    private void SetupBronzeMedalMatch()
    {
        if (BronzeMedalMatch == null || Rounds.Count < 2)
            return;

        // Find the semi-final round (second to last round)
        var semiFinalRound = Rounds[Rounds.Count - 2];

        // Ensure both semi-final matches are complete before setting up bronze medal match
        if (!semiFinalRound.Matches.All(m => m.Winner != null))
        {
            return; // Not all semi-finals are complete yet
        }

        // Get the losers from the semi-final matches
        var semiFinalists = new List<Player>();
        foreach (var match in semiFinalRound.Matches)
        {
            // Add the loser to the bronze medal match
            var loser = match.Player1 == match.Winner ? match.Player2 : match.Player1;
            if (loser != null)
            {
                semiFinalists.Add(loser);
            }
        }

        // Only assign if we have exactly 2 losers and the bronze medal match is empty
        if (semiFinalists.Count == 2 && BronzeMedalMatch.Player1 == null && BronzeMedalMatch.Player2 == null)
        {
            BronzeMedalMatch.Player1 = semiFinalists[0];
            BronzeMedalMatch.Player2 = semiFinalists[1];
        }
    }
}
