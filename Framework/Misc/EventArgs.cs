using Framework.Engine;
using Microsoft.Xna.Framework;

namespace Framework.Misc;

/// <summary>
/// Card related <see cref="EventArgs"/> holding event information of a <see cref="TraditionalCard"/>.
/// </summary>
public class CardEventArgs : EventArgs
{
    public TraditionalCard Card { get; set; }
    public CardEventArgs(TraditionalCard card) =>
        Card = card;
}

public class PositionChangedEventArgs : EventArgs
{
    public Vector2 PrevPosition { get; }
    public Vector2 NewPosition { get; }
    public PositionChangedEventArgs(Vector2 prevPosition, Vector2 newPosition)
    {
        PrevPosition = prevPosition;
        NewPosition = newPosition;
    }
}

public class TextChangedEventArgs : EventArgs
{
    public string PrevText { get; }
    public string NewText { get; }
    public TextChangedEventArgs(string prevText, string newText)
    {
        PrevText = prevText;
        NewText = newText;
    }
}