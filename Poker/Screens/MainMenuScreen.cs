using Microsoft.Xna.Framework;
using Poker.UI;
using System;

namespace Poker.Screens;

/// <summary>
/// Main menu screen shown before the game starts.
/// </summary>
class MainMenuScreen : MenuScreen
{
    public MainMenuScreen(ScreenManager screenManager) : base(screenManager, 3)
    { }

    public override void Initialize()
    {
        // calculate button positions
        int buttonCount = 3;
        int x = (Constants.GameWidth - buttonCount * Constants.ButtonWidthWithMargin 
            + Constants.SpaceBetweenButtons) / 2;

        // create buttons
        Button start = new(Constants.ButtonPlayText, x, Game);
        Button theme = new(Constants.ButtonThemeText, x + Constants.ButtonWidthWithMargin, Game);
        Button exit = new(Constants.ButtonExitText, x + Constants.ButtonWidthWithMargin * 2, Game);

        // click handlers
        start.Click += StartButton_OnClick;
        exit.Click += ExitButton_OnClick;

        // save button references
        Buttons.AddRange(new Button[] { start, theme, exit });

        base.Initialize();
    }

    protected override void LoadContent()
    {
        Texture = Art.PokerTitle;
        base.LoadContent();
    }

    protected override void OnVisibleChanged(object sender, EventArgs args)
    {
        base.OnVisibleChanged(sender, args);

        // show buttons
        if (Visible)
        {
            foreach (var button in Buttons)
                button.Visible = Visible;
        }
    }

    protected override void OnEnabledChanged(object sender, EventArgs args)
    {
        base.OnEnabledChanged(sender, args);

        // hide buttons
        if (Enabled)
        {
            foreach (var button in Buttons)
                button.Enabled = Enabled;
        }
    }

    void StartButton_OnClick(object o, EventArgs e) =>
        ScreenManager.ShowScreen<GameplayScreen>();

    void ExitButton_OnClick(object o, EventArgs e) =>
        Game.Exit();
}