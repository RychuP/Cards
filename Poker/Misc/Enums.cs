namespace Poker.Misc;

/// <summary>
/// Possible states of the onscreen buttons.
/// </summary>
enum PokerButtonState
{
    Normal,
    Hover,
    Pressed
}

/// <summary>
/// Used for player classes.
/// </summary>
public enum Gender {
    Male,
    Female
}

/// <summary>
/// The various possible game states.
/// </summary>
public enum GameState
{
    None,
    Shuffling,
    Dealing,
    FirstBet,
    Turn,
    SecondBet,
    River,
    ThirdBet,
    RoundEnd,
    GameOver
}

public enum BlindType
{
    Small,
    Big
}