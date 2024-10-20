using Framework.Engine;
using Framework.Misc;
using Microsoft.Xna.Framework.Graphics;
using Solitaire.Gameplay.Piles;
using Solitaire.Managers;
using Solitaire.Misc;
using System;
using System.Linq;

namespace Solitaire.UI.AnimatedPiles;

internal class AnimatedWaste : AnimatedPile
{
    public AnimatedWaste(Waste waste) : base(waste)
    { }

    public override Vector2 GetCardRelativePosition(int cardLocationInHand)
    {
        int index = cardLocationInHand;
        if (Hand.Count > 3)
        {
            int lastIndex = Hand.Count - 1;
            int diff = lastIndex - cardLocationInHand;
            index = diff == 2 ? 0 : diff == 1 ? 1 : diff == 0 ? 2 : 0;
        }
        return new Vector2(Pile.CardSpacing.X * index, 0);
    }

    public override TraditionalCard GetCardFromPosition(Point position)
    {
        if (Pile.Count > 0)
        {
            var animCard = AnimatedCards.Last();
            var bounds = new Rectangle(animCard.Position.ToPoint(), TraditionalCard.Size);
            if (bounds.Contains(position))
                return animCard.Card;
        }
        return null;
    }

    protected override void Hand_OnCardReceived(object o, CardEventArgs e)
    {
        base.Hand_OnCardReceived(o, e);
        var animCard = GetCardGameComponent(e.Card);
        animCard.IsFaceDown = false;
    }
}