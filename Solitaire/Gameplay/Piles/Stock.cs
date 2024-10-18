using Solitaire.Misc;
using Solitaire.Managers;
using System.Collections.Generic;
using Solitaire.UI.Screens;
using Solitaire.UI.AnimatedGameComponents;
using Framework.Engine;

namespace Solitaire.Gameplay.Piles;

internal class Stock : Pile
{
    /// <summary>
    /// Raised when there are no cards left in this pile.
    /// </summary>
    public event EventHandler IsEmpty;

    public bool CardsDealt { get; private set; }

    public Stock(GameManager gm) : base(gm, Place.Stock, true)
    {
        AnimatedPile = new AnimatedPile(this);

        // register event handlers
        gm.InputManager.Click += InputManager_OnClick;
        gm.ScreenManager.ScreenChanged += ScreenManager_OnScreenChanged;   
    }

    void ScreenManager_OnScreenChanged(object o, ScreenChangedEventArgs e)
    {
        switch (e.NewScreen)
        {
            case GameplayScreen:
                if (e.PrevScreen is StartScreen)
                {
                    Shuffle();
                    CardsDealt = false;
                    DealTablueaCards();
                    CardsDealt = true;
                }
                break;
        }
    }

    void DealTablueaCards()
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

    void InputManager_OnClick(object o, ClickEventArgs e)
    {
        if (Bounds.Contains(e.Position))
            DealWasteCards();
    }
}