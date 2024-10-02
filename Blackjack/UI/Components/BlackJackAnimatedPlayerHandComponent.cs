using Framework.Engine;
using Framework.UI;
using Microsoft.Xna.Framework;

namespace Blackjack.UI.Components;

public class BlackjackAnimatedPlayerHandComponent : AnimatedHandGameComponent
{
    Vector2 _offset;

    /// <summary>
    /// Creates a new instance of the 
    /// <see cref="BlackjackAnimatedPlayerHandComponent"/> class.
    /// </summary>
    /// <param name="place">A number indicating the hand's position on the 
    /// game table.</param>
    /// <param name="hand">The player's hand.</param>
    /// <param name="cardGame">The associated game.</param>
    public BlackjackAnimatedPlayerHandComponent(int place, Hand hand, CardGame cardGame)
        : base(place, hand, cardGame) =>
        _offset = Vector2.Zero;

    /// <summary>
    /// Creates a new instance of the 
    /// <see cref="BlackjackAnimatedPlayerHandComponent"/> class.
    /// </summary>
    /// <param name="place">A number indicating the hand's position on the 
    /// game table.</param>
    /// <param name="hand">The player's hand.</param>
    /// <param name="cardGame">The associated game.</param>
    /// <param name="offset">An offset which will be added to all card locations
    /// returned by this component.</param>
    public BlackjackAnimatedPlayerHandComponent(int place, Vector2 offset, Hand hand, CardGame cardGame)
        : base(place, hand, cardGame) =>
        _offset = offset;

    /// <summary>
    /// Gets the position relative to the hand position at which a specific card
    /// contained in the hand should be rendered.
    /// </summary>
    /// <param name="cardLocationInHand">The card's location in the hand (0 is the
    /// first card in the hand).</param>
    /// <returns>An offset from the hand's location where the card should be 
    /// rendered.</returns>
    public override Vector2 GetCardRelativePosition(int cardLocationInHand) =>
        new Vector2(25 * cardLocationInHand, -30 * cardLocationInHand) + _offset;
}