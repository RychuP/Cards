#region File Description
//-----------------------------------------------------------------------------
// PauseScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using GameStateManagement;
using Microsoft.Xna.Framework;

namespace Blackjack;

class PauseScreen : MenuScreen
{
    #region Initializations
    /// <summary>
    /// Initializes a new instance of the screen.
    /// </summary>
    public PauseScreen() : base("") { }
    #endregion

    public override void LoadContent()
    {
        // Create our menu entries.
        MenuEntry returnGameMenuEntry = new("Back");
        MenuEntry exitMenuEntry = new("Quit");

        // Hook up menu event handlers.
        returnGameMenuEntry.Selected += OnReturnGameMenuEntrySelected;
        exitMenuEntry.Selected += ExitButtton_OnSelected;

        // Add entries to the menu.
        MenuEntries.Add(returnGameMenuEntry);
        MenuEntries.Add(exitMenuEntry);

        base.LoadContent();
    }

    #region Update
    /// <summary>
    /// Respond to "Return" Item Selection
    /// </summary>
    void OnReturnGameMenuEntrySelected(object sender, EventArgs e)
    {
        GameScreen[] screens = ScreenManager.GetScreens();
        GameplayScreen gameplayScreen = null;
        List<GameScreen> res = new();

        for (int screenIndex = 0; screenIndex < screens.Length; screenIndex++)
        {
            if (screens[screenIndex] is GameplayScreen screen)
                gameplayScreen = screen;
            else
                res.Add(screens[screenIndex]);
        }

        foreach (GameScreen screen in res)
            screen.ExitScreen();

        gameplayScreen.ReturnFromPause();
    }

    /// <summary>
    /// Respond to "Quit Game" Item Selection
    /// </summary>
    /// <param name="playerIndex"></param>
    protected override void OnCancel(PlayerIndex playerIndex)
    {
        Environment.Exit(0);

        for (int componentIndex = 0; componentIndex < ScreenManager.Game.Components.Count; componentIndex++)
        {
            if (!(ScreenManager.Game.Components[componentIndex] is ScreenManager))
            {
                if (ScreenManager.Game.Components[componentIndex] is DrawableGameComponent)
                {
                    (ScreenManager.Game.Components[componentIndex] as IDisposable).Dispose();
                    componentIndex--;
                }
                else
                {
                    ScreenManager.Game.Components.RemoveAt(componentIndex);
                    componentIndex--;
                }
            }
        }

        foreach (GameScreen screen in ScreenManager.GetScreens())
            screen.ExitScreen();

        ScreenManager.AddScreen(new BackgroundScreen(), null);
        ScreenManager.AddScreen(new MainMenuScreen(), null);
    }
    #endregion
}
