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
    Evaluation,
    RoundEnd,
    GameOver,
    Waiting
}

public enum PlayerState
{
    Waiting,

    Checked,
    Called,
    Raised,

    AllIn,
    Folded,
    Bankrupt,
    Winner
}

/// <summary>
/// Used in betting rounds to signify whether checks are possible.
/// </summary>
public enum BetStage
{
    Check,
    Raise
}

/// <summary>
/// All possible poker hands.
/// </summary>
public enum PokerHand
{
    HighCard,
    OnePair,
    TwoPair,
    ThreeOfKind,
    Straight,
    Flush,
    FullHouse,
    FourOfKind,
    StraightFlush,
    RoyalFlush
}

public enum ShufflingType
{
    Ordered, // for debugging purposes
    Random
}