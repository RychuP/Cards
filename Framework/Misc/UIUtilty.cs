using Framework.Engine;

namespace Framework.Misc;

public static class UIUtilty
{
    /// <summary>
    /// Gets the name of a card asset.
    /// </summary>
    /// <param name="card">The card type for which to get the asset name.</param>
    /// <returns>The card's asset name.</returns>
    public static string GetCardAssetName(TraditionalCard card)
    {
        return string.Format("{0}{1}",
            (card.Value | CardValues.FirstJoker) ==
                CardValues.FirstJoker ||
            (card.Value | CardValues.SecondJoker) ==
            CardValues.SecondJoker ?
                "" : card.Type.ToString(), card.Value);
    }
}
