using Poker.UI;
using System;

namespace Poker.Screens;

/// <summary>
/// Main menu screen shown when the game loads.
/// </summary>
internal class MainMenuScreen : MenuScreen
{
    public MainMenuScreen() : base("title", 3)
    { }

    public override void Initialize()
    {
        // calculate button positions
        int buttonCount = 3;
        int x = (Destination.Width - buttonCount * Button.Width - (buttonCount - 1) * ButtonSpacer) / 2;

        // create buttons
        Button start = new("Play", x, ButtonRow, ScreenManager.Game);
        Button theme = new("Theme", x + Button.Width + ButtonSpacer, ButtonRow, ScreenManager.Game);
        Button exit = new("Exit", x + (Button.Width + ButtonSpacer) * 2, ButtonRow, ScreenManager.Game);

        // click handlers
        start.Click += StartButton_OnClick;
        exit.Click += ExitButton_OnClick;

        // save button references
        Buttons.AddRange(new Button[] { start, theme, exit });

        base.Initialize();
    }

    void StartButton_OnClick(object o, EventArgs e) =>
        ScreenManager.ShowScreen<GameplayScreen>();

    void ExitButton_OnClick(object o, EventArgs e) =>
        ScreenManager.Game.Exit();
}