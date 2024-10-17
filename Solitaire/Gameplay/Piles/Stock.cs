using Solitaire.Misc;
using Solitaire.Managers;
using Framework.Misc;
using Framework.Engine;

namespace Solitaire.Gameplay.Piles;

internal class Stock : Pile
{
    public Stock(GameManager gm) : base(gm, Place.Stock, PileOrientation.Up, true)
    {

    }
}