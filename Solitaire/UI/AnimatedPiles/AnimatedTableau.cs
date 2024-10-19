using Framework.Engine;
using Framework.Misc;
using Solitaire.Gameplay.Piles;
using Solitaire.Misc;
using Solitaire.UI.Screens;
using System.Collections.Generic;
using System.Linq;

namespace Solitaire.UI.AnimatedGameComponents;

internal class AnimatedTableau : AnimatedPile
{
    static readonly Point ReducedCardSize = new(TraditionalCard.Width, Pile.CardSpacing.Y);

    public AnimatedTableau(Tableau tableau) : base(tableau)
    {
        Hand.CardRemoved += Hand_OnCardRemoved;
    }

    // tableau cards stack down with gaps between each card
    public override Vector2 GetCardRelativePosition(int cardLocationInHand) =>
        new(0, Pile.CardSpacing.Y * cardLocationInHand);

    /// <summary>
    /// Sets all the opening cards face down apart from the last one.
    /// </summary>
    public void SetUpCards()
    {
        foreach (var animCard in AnimatedCards)
            animCard.IsFaceDown = true;
        AnimatedCards.Last().IsFaceDown = false;
    }

    /// <summary>
    /// Searches the pile to find the card whose visible part contains the given position.
    /// </summary>
    /// <param name="position">Usually the position from the mouse click.</param>
    /// <remarks>Only face up cards are taken into consideration.</remarks>
    /// <returns>Card if found, otherwise null.</returns>
    public TraditionalCard GetCardFromScreenPosition(Point position)
    {
        var lastAnimCard = AnimatedCards.Last();
        for (int i = 0; i < Hand.Count; i++)
        {
            var card = Hand[i];
            var animCard = GetCardGameComponent(card);
            var size = animCard == lastAnimCard ? TraditionalCard.Size : ReducedCardSize;
            var bounds = new Rectangle(animCard.Position.ToPoint(), size);

            if (bounds.Contains(position))
            {
                if (!animCard.IsFaceDown)
                    return card;
                else
                    break;
            }
        }
        return null;
    }

    void Hand_OnCardRemoved(object o, CardEventArgs e)
    {
        if (Hand.Count > 0)
        {
            var animCard = AnimatedCards.Last();
            if (animCard.IsFaceDown)
                animCard.IsFaceDown = false;
        }
    }

    protected override void Hand_OnCardReceived(object o, CardEventArgs e)
    {
        base.Hand_OnCardReceived(o, e);
        var animCard = GetCardGameComponent(e.Card);
        animCard.IsFaceDown = false;
    }
}