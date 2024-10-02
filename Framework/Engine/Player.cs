namespace Framework.Engine;

/// <summary>
/// Represents base player to be extended by inheritance for each card game.
/// </summary>
public class Player
{
    #region Property
    public string Name { get; set; }
    public CardGame Game { get; set; }
    public Hand Hand { get; set; }
    #endregion

    public Player(string name, CardGame game)
    {
        Name = name;
        Game = game;
        Hand = new Hand();
    }
}