using Framework.Misc;
using Solitaire.Gameplay.Piles;

namespace Solitaire.UI.AnimatedPiles;

internal class AnimatedFoundation : AnimatedPile
{
    public AnimatedFoundation(Foundation foundation) : base(foundation)
    {

    }

    protected override void Hand_OnCardReceived(object o, CardEventArgs e)
    {
        base.Hand_OnCardReceived(o, e);
        var animCard = GetCardGameComponent(e.Card);
        animCard.IsFaceDown = false;
    }
}