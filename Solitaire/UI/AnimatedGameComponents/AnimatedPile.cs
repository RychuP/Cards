using Framework.UI;
using Microsoft.Xna.Framework.Graphics;
using Solitaire.Gameplay.Piles;
using Solitaire.Managers;
using Solitaire.Misc;
using Solitaire.UI.Screens;

namespace Solitaire.UI.AnimatedGameComponents;

internal class AnimatedPile : AnimatedHandGameComponent
{
    public static readonly int Spacing = 30;
    public static readonly int VerticalCardSpacing = 33;
    public static readonly int HorizontalCardSpacing = 31;
    public static readonly int OutlineWidth = 93;
    public static readonly int OutlineHeight = 118;

    /// <summary>
    /// Which way the cards are laid out.
    /// </summary>
    public PileOrientation Orientation { get; }

    /// <summary>
    /// Whethere the outline of the base card should be displayed or not.
    /// </summary>
    public bool OutlineVisible { get; }

    public AnimatedPile(Pile pile, PileOrientation orientation, bool outlineVisible) 
        : base((int)pile.Place, pile, pile.GameManager)
    {
        Orientation = orientation;
        OutlineVisible = outlineVisible;

        pile.GameManager.Game.Components.Add(this);
        Hide();

        var gameplayScreen = pile.GameManager.ScreenManager.GetScreen<GameplayScreen>();
        gameplayScreen.VisibleChanged += GameplayScreen_OnVisibleChanged;
    }

    public override void Draw(GameTime gameTime)
    {
        if (OutlineVisible)
        {
            var sb = Game.Services.GetService<SpriteBatch>();
            sb.Begin();
            sb.Draw(Art.CardOutline, Position, Color.White);
            sb.End();
        }

        base.Draw(gameTime);
    }

    public void Show()
    {
        Visible = true;
        Enabled = true;
    }

    public void Hide()
    {
        Visible = false;
        Enabled = false;
    }

    void GameplayScreen_OnVisibleChanged(object o, EventArgs e)
    {
        if (o is not GameplayScreen gameplayScreen) return;
        if (gameplayScreen.Visible == true)
            Show();
        else
            Hide();
    }
}