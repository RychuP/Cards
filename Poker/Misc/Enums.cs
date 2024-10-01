namespace Poker.Misc;

enum PokerButtonState
{
    Normal,
    Hover,
    Pressed
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