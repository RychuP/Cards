namespace Framework.Engine;

/// <summary>
/// Represents base player to be extended by inheritance for each card game.
/// </summary>
public class Player
{
    public string Name { get; set; }
    public CardGame CardGame { get; set; }
    public Hand Hand { get; set; }

    public Player(string name, CardGame game)
    {
        Name = name;
        CardGame = game;
        Hand = new Hand();
    }
}