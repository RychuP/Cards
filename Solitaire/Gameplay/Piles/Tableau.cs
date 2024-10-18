using Framework.Engine;
using Framework.Misc;
using Solitaire.Managers;
using Solitaire.Misc;
using Solitaire.UI;
using Solitaire.UI.AnimatedGameComponents;
using Solitaire.UI.BaseScreens;
using Solitaire.UI.Screens;
using System.Linq;

namespace Solitaire.Gameplay.Piles;

internal class Tableau : Pile
{
    public Tableau(GameManager gm, Place place) : base(gm, place, true)
    {
        Point size = new(Bounds.Width, SolitaireGame.Height - (int)Position.Y - GameScreen.HorizontalMargin);
        Bounds = new Rectangle(Bounds.Location, size);
        AnimatedPile = new AnimatedTableau(this);

        // register event handlers
        GameManager.InputManager.Click += InputManager_OnClick;
    }

    public bool CanReceiveCard(TraditionalCard card)
    {
        if (Count == 0 && card.Value == CardValues.King)
            return true;
        else if (card.Value == CardValues.Ace)
            return false;
        else if (Count > 0)
        {
            var lastCard = Cards.Last();
            bool cardsAreOppositeColors = GameManager.CheckOppositeColor(card, lastCard);
            bool cardsHaveConsecutiveValue = GameManager.CheckConsecutiveValue(card, lastCard);
            if (cardsAreOppositeColors && cardsHaveConsecutiveValue) 
                return true;
        }
        return false;
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