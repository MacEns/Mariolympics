namespace Mariolympics.Models;

public class Player
{
    public int Id { get; set; }

    public Person Person { get; set; }
    public int PersonId { get; set; }

    public string CharacterName { get; set; }
    public Character Character => Character.Get(CharacterName);

    public override string ToString() => Person?.FullName;
}
