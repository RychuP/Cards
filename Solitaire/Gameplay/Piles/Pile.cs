﻿using Framework.Engine;
using Solitaire.Managers;
using Solitaire.Misc;
using Solitaire.UI;
using Solitaire.UI.AnimatedGameComponents;
using Solitaire.UI.Screens;

namespace Solitaire.Gameplay.Piles;

internal class Pile : Hand
{
    public static readonly Point OutlineSpacing = new(30, 50);
    public static readonly Point CardSpacing = new(31, 33);
    public static readonly int OutlineWidth = 93;
    public static readonly int OutlineHeight = 118;

    public AnimatedPile AnimatedPile { get; init; }
    public GameManager GameManager { get; }
    public Game Game => GameManager.Game;
    public Vector2 Position { get; }

    /// <summary>
    /// Whethere the outline of the base card should be displayed or not.
    /// </summary>
    public bool OutlineVisible { get; }

    /// <summary>
    /// The screen area that the pile monitors for clicks and drags.
    /// </summary>
    public Rectangle Bounds { get; init; }

    /// <summary>
    /// Place that the pile occupies on the table.
    /// </summary>
    public PilePlace Place { get; }

    public Pile(GameManager gm, PilePlace place, bool outlineVisible)
    {
        Place = place;
        GameManager = gm;
        Position = gm.GameTable[(int)place];
        OutlineVisible = outlineVisible;
        Bounds = new Rectangle(Position.ToPoint(), Art.CardOutline.Bounds.Size);

        // register event handlers
        gm.ScreenManager.ScreenChanged += ScreenManager_OnScreenChanged;
    }

    protected void ReturnCardsToStock() =>
        DealCardsToHand(GameManager.Stock, Count);

    /// <summary>
    /// Called from animated pile when the left mouse button is released.
    /// </summary>
    /// <param name="pile"></param>
    /// <param name="startCard"></param>
    public virtual void DropCards(Pile pile, TraditionalCard startCard) { }

    protected virtual void OnClick(Point position) { }

    void ScreenManager_OnScreenChanged(object o, ScreenChangedEventArgs e)
    {
        switch (e.NewScreen)
        {
            case StartScreen:
                // return the cards to stock when the game ends
                if (e.PrevScreen is PauseScreen && this is not Stock)
                    ReturnCardsToStock();
                break;

            case WinScreen:
                // return the cards to stock when the game ends
                if (e.PrevScreen is GameplayScreen && this is not Stock)
                    ReturnCardsToStock();
                break;
        }
    }
}