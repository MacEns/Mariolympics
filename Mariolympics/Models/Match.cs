namespace Mariolympics.Models;

public class Match
{
    public int Id { get; set; }

    public int RoundId { get; set; }
    public Round Round { get; set; }

    public Player Player1 { get; set; }
    public Player Player2 { get; set; }

    public Player Winner { get; set; }
}
