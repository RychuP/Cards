using Solitaire.Misc;
using Solitaire.Managers;
using Solitaire.UI.AnimatedGameComponents;
using Framework.Engine;
using System.Linq;
using Framework.Misc;

namespace Solitaire.Gameplay.Piles;

internal class Foundation : Pile
{
    public Foundation(GameManager gm, Place place) : base(gm, place, true)
    {
        AnimatedPile = new AnimatedFoundation(this);
    }

    public bool CanReceiveCard(TraditionalCard card)
    {
        if (Count == 0 && card.Value == CardValues.Ace)
            return true;
        else if (Count > 0 && card.Type == this[0].Type && 
            GameManager.CheckConsecutiveValue(Cards.Last(), card))
            return true;
        return false;
    }
}