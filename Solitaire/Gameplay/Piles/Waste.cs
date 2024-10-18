using Framework.Engine;
using Framework.Misc;
using Solitaire.Managers;
using Solitaire.Misc;
using Solitaire.UI.AnimatedGameComponents;
using System.Collections.Generic;
using System.Linq;

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
        GameManager.InputManager.Click += InputManager_OnClick;
    }

    void Stock_OnIsEmpty(object o, EventArgs e)
    {
        while (Count > 0)
            DealCardToHand(GameManager.Stock);
    }

    void InputManager_OnClick(object o, ClickEventArgs e)
    {
        if (Bounds.Contains(e.Position) && Count > 0)
        {
            var card = Cards.Last();

            // check foundations first
            foreach (var foundation in GameManager.Foundations)
            {
                if (foundation.CanReceiveCard(card)) 
                {
                    card.MoveToHand(foundation);
                    return;
                }
            }

            // check tableaus
            foreach (var tableau in GameManager.Tableaus)
            {
                if (tableau.CanReceiveCard(card))
                {
                    card.MoveToHand(tableau);
                    return;
                }
            }
        }
    }
}