using System;
using Blackjack.UI.ScreenElements;
using Microsoft.Xna.Framework;

namespace Blackjack.UI.Screens;

class MainMenuScreen : MenuScreen
{
    public static string Theme = "Red";

    /// <summary>
    /// Initializes a new instance of the screen.
    /// </summary>
    public MainMenuScreen() : base("") { }

    public override void LoadContent()
    {
        // Create our menu entries.
        MenuEntry startGameMenuEntry = new("Play");
        MenuEntry themeGameMenuEntry = new("Theme");
        MenuEntry exitMenuEntry = new("Exit");

        // Hook up menu event handlers.
        startGameMenuEntry.Selected += StartButton_OnSelected;
        themeGameMenuEntry.Selected += ThemeButton_OnSelected;
        exitMenuEntry.Selected += ExitButtton_OnSelected;

        // Add entries to the menu.
        MenuEntries.Add(startGameMenuEntry);
        MenuEntries.Add(themeGameMenuEntry);
        MenuEntries.Add(exitMenuEntry);

        base.LoadContent();
    }

    /// <summary>
    /// Respond to "Play" Item Selection
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void StartButton_OnSelected(object sender, EventArgs e)
    {
        foreach (GameScreen screen in ScreenManager.GetScreens())
            screen.ExitScreen();

        ScreenManager.AddScreen(new GameplayScreen(Theme), null);
    }

    /// <summary>
    /// Respond to "Theme" Item Selection
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void ThemeButton_OnSelected(object sender, EventArgs e)
    {
        ScreenManager.AddScreen(new OptionsMenu(), null);
    }

    /// <summary>
    /// Respond to "Exit" Item Selection
    /// </summary>
    /// <param name="playerIndex"></param>
    protected override void OnCancel(PlayerIndex playerIndex)
    {
        ScreenManager.Game.Exit();
    }
}
