namespace Mariolympics.Models;

public class Round
{
    public int Id { get; set; }

    public int BracketId { get; set; }
    public Bracket Bracket { get; set; }

    public int RoundNumber { get; set; }

    public ICollection<Match> Matches { get; set; } = new List<Match>();
}
