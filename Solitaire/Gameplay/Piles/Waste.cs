using Solitaire.Managers;
using Solitaire.Misc;

namespace Solitaire.Gameplay.Piles;

internal class Waste : Pile
{

    public Waste(GameManager gm) : base(gm, Place.Waste, PileOrientation.Down, false)
    {

    }
}