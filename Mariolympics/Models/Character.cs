namespace Mariolympics.Models;

public class Character
{
    private Character(string value) { Value = value; }

    public string Value { get; private set; }

    public static Character Mario => new("Mario");
    public static Character Luigi => new("Luigi");
    public static Character Peach => new("Peach");
    public static Character Daisy => new("Daisy");
    public static Character Wario => new("Wario");
    public static Character Waluigi => new("Waluigi");
    public static Character Yoshi => new("Yoshi");
    public static Character DonkeyKong => new("DonkeyKong");

    public static List<Character> All => new() { Mario, Luigi, Peach, Daisy, Wario, Waluigi, Yoshi, DonkeyKong };

    public override string ToString()
    {
        return Value;
    }

    public Character Get(string name) => name switch
    {
        "Mario" => Mario,
        "Luigi" => Luigi,
        "Peach" => Peach,
        "Daisy" => Daisy,
        "Wario" => Wario,
        "Waluigi" => Waluigi,
        "Yoshi" => Yoshi,
        "DonkeyKong" => DonkeyKong,
        _ => new("error"),
    };
}
