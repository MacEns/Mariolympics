namespace Mariolympics.Models;

public class Game
{
    private Game(string value) { Value = value; }

    public string Value { get; private set; }

    public static Game Tennis => new("Tennis");
    public static Game Golf => new("Golf");
    public static Game Strikers => new("Strikers");
    public static Game Baseball => new("Baseball");
    public static Game Kart => new("Kart");

    public override string ToString()
    {
        return Value;
    }
}
