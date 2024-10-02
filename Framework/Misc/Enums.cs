namespace Framework.Misc;

/// <summary>
/// Enum defining the various types of cards for a traditional-western card-set
/// </summary>
[Flags]
public enum CardSuits
{
    Heart = 0x01,
    Diamond = 0x02,
    Club = 0x04,
    Spade = 0x08,

    // Sets:
    AllSuits = Heart | Diamond | Club | Spade
}

/// <summary>
/// Enum defining the various types of card values for a traditional-western card-set
/// </summary>
[Flags]
public enum CardValues
{
    Ace = 0x01,
    Two = 0x02,
    Three = 0x04,
    Four = 0x08,
    Five = 0x10,
    Six = 0x20,
    Seven = 0x40,
    Eight = 0x80,
    Nine = 0x100,
    Ten = 0x200,
    Jack = 0x400,
    Queen = 0x800,
    King = 0x1000,
    FirstJoker = 0x2000,
    SecondJoker = 0x4000,

    // Sets:
    AllNumbers = 0x3FF,
    NonJokers = 0x1FFF,
    Jokers = FirstJoker | SecondJoker,
    AllFigures = Jack | Queen | King,
}

public enum FontType
{
    regular,
    italic,
    bold,
    underline,
}