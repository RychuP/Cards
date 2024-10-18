using Framework.Misc;
using Solitaire.Gameplay.Piles;

namespace Solitaire.UI.AnimatedGameComponents;

internal class AnimatedTableau : AnimatedPile
{
    public AnimatedTableau(Tableau tableau) : base(tableau)
    {

    }

    public override Vector2 GetCardRelativePosition(int cardLocationInHand) =>
        new(0, Pile.CardSpacing.Y * cardLocationInHand);

    protected override void Hand_OnCardReceived(object o, CardEventArgs e)
    {
        base.Hand_OnCardReceived (o, e);

        var newTopCard = GetCardGameComponent(e.Card);
        newTopCard.IsFaceDown = false;

        if (Hand.Count > 1)
        {
            var index = GetCardLocationInHand(e.Card);
            var prevTopCard = GetCardGameComponent(index - 1); 
            prevTopCard.IsFaceDown = true;
        }
    }
}