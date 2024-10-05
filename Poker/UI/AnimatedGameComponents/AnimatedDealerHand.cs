using Framework.Engine;
using Framework.UI;

namespace Poker.UI.AnimatedGameComponents;

class AnimatedDealerHand : AnimatedHandGameComponent
{
    /// <summary>
    /// Creates a new instance of the 
    /// <see cref="AnimatedPlayerHand"/> class.
    /// </summary>
    /// <param name="place">A number indicating the hand's position on the 
    /// game table.</param>
    /// <param name="hand">The player's hand.</param>
    /// <param name="cardGame">The associated game.</param>
    public AnimatedDealerHand(int place, Hand hand, CardGame cardGame)
        : base(place, hand, cardGame)
    { }

    /// <summary>
    /// Gets the position relative to the hand position at which a specific card
    /// contained in the hand should be rendered.
    /// </summary>
    /// <param name="cardLocationInHand">The card's location in the hand (0 is the
    /// first card in the hand).</param>
    /// <returns>An offset from the hand's location where the card should be 
    /// rendered.</returns>
    public override Vector2 GetCardRelativePosition(int cardLocationInHand)
    {
        int offsetX = (Constants.CardSize.X + Constants.CommunityCardPadding) * cardLocationInHand;
        return new Vector2(offsetX, 0);
    }
}