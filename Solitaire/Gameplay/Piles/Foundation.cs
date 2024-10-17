using Solitaire.Misc;
using Solitaire.Managers;

namespace Solitaire.Gameplay.Piles;

internal class Foundation : Pile
{
    public Foundation(GameManager gm, Place place) : base(gm, place, PileOrientation.Up, true)
    {

    }
}