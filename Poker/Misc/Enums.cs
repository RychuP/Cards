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
    Preflop,
    Flop,
    FlopBet,
    Turn,
    TurnBet,
    River,
    RiverBet,
    Showdown,
    RoundEnd,
    GameOver
}

public enum PlayerState
{
    Waiting,
    Checked,
    Called,
    Raised,
    AllIn,
    Folded,
    Bankrupt
}

public enum BetStage
{
    Check,
    Raise
}