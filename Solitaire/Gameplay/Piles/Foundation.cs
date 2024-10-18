using Solitaire.Misc;
using Solitaire.Managers;
using Solitaire.UI.AnimatedGameComponents;

namespace Solitaire.Gameplay.Piles;

internal class Foundation : Pile
{
    public Foundation(GameManager gm, Place place) : base(gm, place, true)
    {
        AnimatedPile = new AnimatedFoundation(this);
    }
}