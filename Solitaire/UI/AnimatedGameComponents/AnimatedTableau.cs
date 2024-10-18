using Framework.Misc;
using Solitaire.Gameplay.Piles;
using System.Linq;

namespace Solitaire.UI.AnimatedGameComponents;

internal class AnimatedTableau : AnimatedPile
{
    public AnimatedTableau(Tableau tableau) : base(tableau)
    {
        Hand.CardRemoved += Hand_OnCardRemoved;
    }

    public override Vector2 GetCardRelativePosition(int cardLocationInHand) =>
        new(0, Pile.CardSpacing.Y * cardLocationInHand);

    protected override void Hand_OnCardReceived(object o, CardEventArgs e)
    {
        base.Hand_OnCardReceived (o, e);
        var newTopCard = GetCardGameComponent(e.Card);
        newTopCard.IsFaceDown = false;

        // flip the initial cards face down
        if (!Pile.GameManager.Stock.CardsDealt)
        {
            if (Hand.Count > 1)
            {
                var index = GetCardLocationInHand(e.Card);
                var prevTopCard = GetCardGameComponent(index - 1);
                prevTopCard.IsFaceDown = true;
            }
        }
    }

    void Hand_OnCardRemoved(object o, CardEventArgs e)
    {
        if (Hand.Count > 0)
        {
            var card = AnimatedCards.Last();
            if (card.IsFaceDown)
                card.IsFaceDown = false;
        }
    }
}