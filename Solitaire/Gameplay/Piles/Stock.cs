using Solitaire.Misc;
using Solitaire.Managers;
using System.Collections.Generic;
using Solitaire.UI.Screens;
using Solitaire.UI.AnimatedPiles;
using Framework.Engine;

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

        // register event handlers
        var startScreen = gm.ScreenManager.GetScreen<StartScreen>();
        gm.InputManager.Click += InputManager_OnClick;
    }

    public void DealTablueaCards()
    {
        int cardsCount = 1;
        foreach (var tableau in GameManager.Tableaus)
            DealCardsToHand(tableau, cardsCount++);
    }

    void DealWasteCards()
    {
        switch (GameManager.Difficulty)
        {
            case Difficulty.Easy:
                DealWasteCard();
                break;

            case Difficulty.Hard:
                for (int i = 0; i < 3; i++)
                {
                    if (!DealWasteCard())
                        break;
                }
                break;
        }
    }

    bool DealWasteCard()
    {
        if (Count > 0)
        {
            DealCardToHand(GameManager.Waste);
            return true;
        }
        else
        {
            OnIsEmpty();
            return false;
        }
    }

    void OnIsEmpty()
    {
        IsEmpty?.Invoke(this, EventArgs.Empty);
    }

    void InputManager_OnClick(object o, PointEventArgs e)
    {
        if (Bounds.Contains(e.Position))
            DealWasteCards();
    }
}