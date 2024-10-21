using Framework.Assets;
using Framework.Engine;
using Solitaire.Managers;
using Solitaire.Misc;
using Solitaire.UI.AnimatedPiles;
using System.Linq;

namespace Solitaire.Gameplay.Piles;

internal class Waste : Pile
{
    public Waste(GameManager gm) : base(gm, PilePlace.Waste, false)
    {
        Point size = new(Bounds.Width + CardSpacing.X * 2, Bounds.Height);
        Bounds = new Rectangle(Bounds.Location, size);

        AnimatedPile = new AnimatedWaste(this);

        // register event handlers
        GameManager.Stock.IsEmpty += Stock_OnIsEmpty;
    }

    public override void DropCards(Pile pile, TraditionalCard startCard)
    {
        if (pile.CanReceiveCard(startCard))
        {
            startCard.MoveToHand(pile);
            CardSounds.Bet.Play();
            OnMoveMade();
        }
    }

    void Stock_OnIsEmpty(object o, EventArgs e)
    {
        if (Count > 0)
        {
            while (Count > 0)
                DealCardToHand(GameManager.Stock);
            OnMoveMade();
        }
    }

    protected override void InputManager_OnClick(object o, PointEventArgs e)
    {
        if (Bounds.Contains(e.Position) && Count > 0)
        {
            var card = Cards.Last();

            // check foundations first
            if (FindCardRecepient(card, GameManager.Foundations))
                return;

            // check tableaus
            if (FindCardRecepient(card, GameManager.Tableaus))
                return;
        }
    }
}