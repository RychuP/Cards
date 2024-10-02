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
public enum PokerGameState
{
    Shuffling,
    Dealing,
    Flop,
    FirstBet,
    Turn,
    SecondBet,
    River,
    ThirdBet,
    RoundEnd,
    GameOver
}