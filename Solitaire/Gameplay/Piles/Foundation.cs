using Solitaire.Misc;
using Solitaire.Managers;
using Solitaire.UI.AnimatedPiles;
using Framework.Engine;
using System.Linq;
using Framework.Misc;
using Framework.Assets;

namespace Solitaire.Gameplay.Piles;

internal class Foundation : Pile
{
    public Foundation(GameManager gm, PilePlace place) : base(gm, place, true)
    {
        AnimatedPile = new AnimatedFoundation(this);
    }

    /// <inheritdoc/>
    public override bool CanReceiveCard(TraditionalCard card)
    {
        if (Count == 0 && card.Value == CardValues.Ace)
            return true;
        // card is the same suit and is next in value to the last card in the pile
        else if (Count > 0 && card.Type == this[0].Type && GameManager.CheckConsecutiveValue(Cards.Last(), card))
            return true;
        return false;
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

    protected override void InputManager_OnClick(object o, PointEventArgs e)
    {
        if (!Bounds.Contains(e.Position) || Count == 0) return;

        // top card can only go back to tableaus
        var card = Cards.Last();
        FindCardRecepient(card, GameManager.Tableaus);
    }
}