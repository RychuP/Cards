using Framework.Assets;
using Framework.Engine;
using Solitaire.Managers;
using Solitaire.Misc;
using Solitaire.UI;
using Solitaire.UI.AnimatedPiles;
using System.Collections.Generic;

namespace Solitaire.Gameplay.Piles;

internal class Pile : Hand
{
    public event EventHandler MoveMade;

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
        GameManager.InputManager.Click += InputManager_OnClick;
        GameManager.GameEnd += GameManager_OnGameEnd;
        GameManager.GameInit += GameManager_OnGameInit;
    }

    protected bool FindCardRecepient<T>(TraditionalCard card, List<T> piles) where T : Pile
    {
        foreach (var pile in piles)
        {
            if (pile.CanReceiveCard(card))
            {
                card.MoveToHand(pile);
                CardSounds.Bet.Play();
                OnMoveMade();
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Called from animated pile when the left mouse button is released.
    /// </summary>
    /// <param name="pile"></param>
    /// <param name="startCard"></param>
    public virtual void DropCards(Pile pile, TraditionalCard startCard) { }

    /// <summary>
    /// Checks if the pile can accept the given card.
    /// </summary>
    /// <param name="card">Card to add.</param>
    /// <returns>True if the card can be added to the pile.</returns>
    public virtual bool CanReceiveCard(TraditionalCard card) => false;

    protected virtual void InputManager_OnClick(object o, PointEventArgs e) { }

    protected virtual void GameManager_OnGameInit(object o, EventArgs e) { }

    protected virtual void GameManager_OnGameEnd(object o, EventArgs e)
    {
        // return cards to stock
        DealCardsToHand(GameManager.Stock, Count);
    }

    protected virtual void OnMoveMade()
    {
        MoveMade?.Invoke(this, EventArgs.Empty);
    }
}