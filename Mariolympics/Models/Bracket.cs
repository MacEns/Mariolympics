namespace Mariolympics.Models;

public class Bracket
{
    public int Id { get; set; }

    public Tournament Tournament { get; set; }
    public int TournamentId { get; set; }

    public string Game { get; set; }

    public ICollection<Round> Rounds { get; set; } = new List<Round>();
}
