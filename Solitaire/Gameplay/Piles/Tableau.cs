using Solitaire.Managers;
using Solitaire.Misc;

namespace Solitaire.Gameplay.Piles;

internal class Tableau : Pile
{
    public Tableau(GameManager gm, Place place) : base(gm, place, PileOrientation.Down, true)
    {

    }
}