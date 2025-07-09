namespace Mariolympics.Models;

public class Tournament
{
    public int Id { get; set; }
    public DateTime Date { get; set; } = DateTime.Now;

    public List<Bracket> Brackets { get; set; } = [];
}
