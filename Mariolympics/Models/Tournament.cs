namespace Mariolympics.Models;

public class Tournament
{
    public int Id { get; set; }
    public DateTime Date { get; set; }

    public List<Bracket> Brackets { get; set; }
}
