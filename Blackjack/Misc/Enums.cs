namespace Blackjack.Misc;

/// <summary>
/// Depicts hands the player can interact with.
/// </summary>
public enum HandTypes
{
    First,
    Second
}

/// <summary>
/// Enum describes the screen transition state.
/// </summary>
public enum ScreenState
{
    TransitionOn,
    Active,
    TransitionOff,
    Hidden,
}

/// <summary>
/// The various possible game states.
/// </summary>
public enum BlackjackGameState
{
    Shuffling,
    Betting,
    Playing,
    Dealing,
    RoundEnd,
    GameOver,
}