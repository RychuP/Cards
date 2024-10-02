using Framework.Engine;

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