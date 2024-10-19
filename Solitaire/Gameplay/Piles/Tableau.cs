using Framework.Engine;
using Framework.Misc;
using Solitaire.Managers;
using Solitaire.Misc;
using Solitaire.UI;
using Solitaire.UI.AnimatedPiles;
using Solitaire.UI.BaseScreens;
using Solitaire.UI.Screens;
using System.Collections.Generic;
using System.Linq;

namespace Solitaire.Gameplay.Piles;

internal class Tableau : Pile
{
    AnimatedTableau AnimatedTablea => AnimatedPile as AnimatedTableau;

    public Tableau(GameManager gm, PilePlace place) : base(gm, place, true)
    {
        Point size = new(Bounds.Width, SolitaireGame.Height - (int)Position.Y - GameScreen.HorizontalMargin);
        Bounds = new Rectangle(Bounds.Location, size);
        AnimatedPile = new AnimatedTableau(this);

        // register event handlers
        gm.InputManager.Click += InputManager_OnClick;
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

    /// <summary>
    /// Moves all cards starting with the given card to another pile.
    /// </summary>
    void MoveCards(Pile pile, TraditionalCard startCard)
    {
        var cardsToMove = new List<TraditionalCard>();

        // find all cards starting with the given card
        for (int i = Cards.Count - 1; i >= 0; i--) 
        {
            var card = Cards[i];
            cardsToMove.Insert(0, card);
            if (card == startCard)
                break;
        }

        // move all found cards to the destination pile
        if (cardsToMove.Count > 0)
        {
            foreach (var card in cardsToMove)
                card.MoveToHand(pile);
        }
    }

    /// <inheritdoc/>
    public override void DropCards(Pile pile, TraditionalCard startCard)
    {
        // foundations can receive only one card at a time
        if (pile is Foundation foundation && startCard == Cards[Count - 1] &&
            foundation.CanReceiveCard(startCard))
        {
            startCard.MoveToHand(foundation);
        }
        else if (pile is Tableau tableau && tableau.CanReceiveCard(startCard))
        {
            MoveCards(tableau, startCard);
        }
    }

    void InputManager_OnClick(object o, PointEventArgs e)
    {
        if (Bounds.Contains(e.Position) && Count > 0)
        {
            // get card from click position
            var card = AnimatedTablea.GetCardFromPosition(e.Position);
            if (card is null) return;

            // foundations can receive only one card at a time
            var lastCard = Cards[Count - 1];
            if (card == lastCard)
            {
                foreach (var foundation in GameManager.Foundations)
                {
                    if (foundation.CanReceiveCard(card))
                    {
                        card.MoveToHand(foundation);
                        return;
                    }
                }
            }
            
            // check tableaus
            foreach (var tableau in GameManager.Tableaus)
            {
                if (tableau.CanReceiveCard(card))
                {
                    MoveCards(tableau, card);
                    return;
                }
            }
        }
    }
}