using Framework.Engine;
using Framework.Misc;
using Solitaire.Managers;
using Solitaire.Misc;
using Solitaire.UI.AnimatedGameComponents;
using System.Collections.Generic;

namespace Solitaire.Gameplay.Piles;

internal class Waste : Pile
{
    public Waste(GameManager gm) : base(gm, Place.Waste, false)
    {
        Point size = new(Bounds.Width + CardSpacing.X * 2, Bounds.Height);
        Bounds = new Rectangle(Bounds.Location, size);

        AnimatedPile = new AnimatedWaste(this);

        // register event handlers
        GameManager.Stock.IsEmpty += Stock_OnIsEmpty;
    }

    void Stock_OnIsEmpty(object o, EventArgs e)
    {
        while (Count > 0)
            DealCardToHand(GameManager.Stock);
    }
}