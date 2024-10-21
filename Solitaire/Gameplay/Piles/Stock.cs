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
        var count = Count;
        int cardsCount = 1;
        foreach (var tableau in GameManager.Tableaus)
            DealCardsToHand(tableau, cardsCount++);
    }

    void DealWasteCards()
    {
        int count = GameManager.GetDrawCount();
        if (Count > 0)
        {
            count = count < Count ? count : Count;
            for (int i = 0; i < count; i++)
                DealCardToHand(GameManager.Waste);
            CardSounds.ShortDeal.Play();
            OnMoveMade();
        }
        else
        {
            OnIsEmpty();
        }
    }

    // stock will not concern itself with the startCard -> it will just deal cards in order
    /// <inheritdoc/>
    public override void DropCards(Pile pile, TraditionalCard startCard)
    {
        if (pile is Waste)
            DealWasteCards();
    }

    void OnIsEmpty()
    {
        IsEmpty?.Invoke(this, EventArgs.Empty);
    }

    protected override void InputManager_OnClick(object o, PointEventArgs e)
    {
        if (Bounds.Contains(e.Position))
            DealWasteCards();
    }

    protected override void GameManager_OnGameInit(object o, EventArgs e)
    {
        Shuffle();
        DealTablueaCards();
    }

    // method cleared as the stock does not need to return cards to itself
    protected override void GameManager_OnGameEnd(object o, EventArgs e) { }
}