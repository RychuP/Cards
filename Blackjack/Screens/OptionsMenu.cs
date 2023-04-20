#region File Description
//-----------------------------------------------------------------------------
// OptionsMenu.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using GameStateManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CardsFramework;

namespace Blackjack;

class OptionsMenu : MenuScreen
{
    readonly Dictionary<string, Texture2D> _themes = new();
    AnimatedGameComponent _card;
    Texture2D _background;
    Rectangle _safeArea;

    #region Initializations
    /// <summary>
    /// Initializes a new instance of the screen.
    /// </summary>
    public OptionsMenu() : base("") { }
    #endregion

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

        _themes.Add("Red", ScreenManager.Game.Content.Load<Texture2D>(
            @"Images\Cards\CardBack_Red"));
        _themes.Add("Blue", ScreenManager.Game.Content.Load<Texture2D>(
            @"Images\Cards\CardBack_Blue"));
        _background = ScreenManager.Game.Content.Load<Texture2D>(
            @"Images\UI\table");

        _card = new AnimatedGameComponent(ScreenManager.Game, _themes[MainMenuScreen.Theme])
        {
            CurrentPosition = new Vector2(_safeArea.Center.X, _safeArea.Center.Y - 50)
        };

        ScreenManager.Game.Components.Add(_card);

        base.LoadContent();
    }

    #region Update and Render
    /// <summary>
    /// Respond to "Theme" Item Selection
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void ThemeButton_OnSelected(object sender, EventArgs e)
    {
        MainMenuScreen.Theme = MainMenuScreen.Theme == "Red" ? "Blue" : "Red";
        _card.CurrentFrame = _themes[MainMenuScreen.Theme];
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
        ScreenManager.SpriteBatch.Begin();

        // Draw the card back
        ScreenManager.SpriteBatch.Draw(_background, ScreenManager.GraphicsDevice.Viewport.Bounds,
            Color.White * TransitionAlpha);

        ScreenManager.SpriteBatch.End();
        base.Draw(gameTime);
    }
    #endregion
}