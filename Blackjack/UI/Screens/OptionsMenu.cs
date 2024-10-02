using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Framework.UI;
using Blackjack.UI.ScreenElements;
using Blackjack.Misc;
using Framework.Assets;

namespace Blackjack.UI.Screens;

class OptionsMenu : MenuScreen
{
    AnimatedGameComponent _card;
    Rectangle _safeArea;

    /// <summary>
    /// Initializes a new instance of the screen.
    /// </summary>
    public OptionsMenu() : base("") { }

    /// <summary>
    /// Loads content required by the screen, and initializes the displayed menu.
    /// </summary>
    public override void LoadContent()
    {
        _safeArea = ScreenManager.SafeArea;

        // Create our menu entries.
        MenuEntry themeGameMenuEntry = new("Deck");
        MenuEntry returnMenuEntry = new("Return");

        // Hook up menu event handlers.
        themeGameMenuEntry.Selected += ThemeButton_OnSelected;
        returnMenuEntry.Selected += ExitButtton_OnSelected;

        // Add entries to the menu.
        MenuEntries.Add(themeGameMenuEntry);
        MenuEntries.Add(returnMenuEntry);

        _card = new AnimatedGameComponent(ScreenManager.Game, CardAssets.CardBacks[MainMenuScreen.Theme])
        {
            Position = new Vector2(_safeArea.Center.X, _safeArea.Center.Y - 50)
        };

        ScreenManager.Game.Components.Add(_card);

        base.LoadContent();
    }

    /// <summary>
    /// Respond to "Theme" Item Selection
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void ThemeButton_OnSelected(object sender, EventArgs e)
    {
        MainMenuScreen.Theme = MainMenuScreen.Theme == "Red" ? "Blue" : "Red";
        _card.Texture = CardAssets.CardBacks[MainMenuScreen.Theme];
    }

    /// <summary>
    /// Respond to "Return" Item Selection
    /// </summary>
    /// <param name="playerIndex"></param>
    protected override void OnCancel(PlayerIndex playerIndex)
    {
        ScreenManager.Game.Components.Remove(_card);
        ExitScreen();
    }

    /// <summary>
    /// Draws the menu.
    /// </summary>
    /// <param name="gameTime"></param>
    public override void Draw(GameTime gameTime)
    {
        var sb = ScreenManager.Game.Services.GetService(typeof(SpriteBatch)) as SpriteBatch;
        sb.Begin();
        sb.Draw(Art.Table, ScreenManager.GraphicsDevice.Viewport.Bounds, Color.White * TransitionAlpha);
        sb.End();
        base.Draw(gameTime);
    }
}