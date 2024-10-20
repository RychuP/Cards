namespace Solitaire.Misc;

public enum PilePlace
{
    Stock,
    Waste,
    Foundation0,
    Foundation1,
    Foundation2,
    Foundation3,
    Tableau0,
    Tableau1,
    Tableau2,
    Tableau3,
    Tableau4,
    Tableau5,
    Tableau6,
}

public enum PileOrientation
{
    Up, 
    Down,
    Right
}

/// <summary>
/// Possible states of the onscreen buttons.
/// </summary>
enum GameButtonState
{
    Normal,
    Hover,
    Pressed
}

public enum Difficulty
{
    Easy, 
    Medium,
    Hard
}

public enum TextAlignment
{
    LeftAligned,
    Centered,
    RightAligned
}

public enum Direction
{
    Up,
    Down,
    Left,
    Right
}

public enum Visibility
{
    Hidden,
    Visible
}