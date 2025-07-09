namespace Mariolympics.Models;

public class Match
{
    public int Id { get; set; }

    public int RoundId { get; set; }
    public Round Round { get; set; }

    public Player Player1 { get; set; }
    public Player Player2 { get; set; }

    public Player Winner { get; set; }

    public void AddPlayer(Player player)
    {
        if (Player1 == null)
        {
            Player1 = player;
        }
        else if (Player2 == null)
        {
            Player2 = player;
        }
        else
        {
            throw new InvalidOperationException("Both players are already assigned to this match.");
        }
    }

    public void SetWinner(Player player)
    {
        if (player == null)
        {
            throw new ArgumentNullException(nameof(player), "Winner cannot be null.");
        }

        if (Player1 == player || Player2 == player)
        {
            Winner = player;
        }
        else
        {
            throw new InvalidOperationException("The specified player is not part of this match.");
        }
    }
}
