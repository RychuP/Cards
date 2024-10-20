using Solitaire.Misc;
using Solitaire.Managers;
using Solitaire.UI.AnimatedPiles;
using Framework.Engine;
using Framework.Assets;

namespace Solitaire.Gameplay.Piles;

internal class Stock : Pile
{
    /// <summary>
    /// Raised when there are no cards left in this pile.
    /// </summary>
    public event EventHandler IsEmpty;

    public Stock(GameManager gm) : base(gm, PilePlace.Stock, true)
    {
        AnimatedPile = new AnimatedPile(this);
    }

    public void DealTablueaCards()
    {
        int cardsCount = 1;
        foreach (var tableau in GameManager.Tableaus)
            DealCardsToHand(tableau, cardsCount++);
    }

    void DealWasteCards(int count)
    {
        for (int i = 0; i < count; i++)
        {
            if (!DealWasteCard())
                break;
        }
    }

    bool DealWasteCard()
    {
        if (Count > 0)
        {
            DealCardToHand(GameManager.Waste);
            CardSounds.ShortDeal.Play();
            return true;
        }
        else
        {
            OnIsEmpty();
            return false;
        }
    }

    public override void DropCards(Pile pile, TraditionalCard startCard)
    {
        if (pile is Waste)
        {
            DealCardToHand(GameManager.Waste);
            CardSounds.Bet.Play();
        }
    }

    void OnIsEmpty()
    {
        IsEmpty?.Invoke(this, EventArgs.Empty);
    }

    protected override void InputManager_OnClick(object o, PointEventArgs e)
    {
        if (Bounds.Contains(e.Position))
        {
            int count = GameManager.GetDrawCount();
            DealWasteCards(count);
        }
    }
}