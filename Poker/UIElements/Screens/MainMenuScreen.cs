using Microsoft.Xna.Framework;
using Poker.Misc;
using System;

namespace Poker.UIElements.Screens;

/// <summary>
/// Main menu screen shown before the game starts.
/// </summary>
class MainMenuScreen : StaticScreen
{
    public MainMenuScreen(ScreenManager screenManager) : base(screenManager, 3)
    { }

    public override void Initialize()
    {
        // calculate left most button position
        int x = (Constants.GameWidth - Buttons.Capacity * Constants.ButtonWidthWithMargin
            + Constants.SpaceBetweenButtons) / 2;

        // create buttons
        Button startButton = new(Constants.ButtonPlayText, x, Game);
        Button themeButton = new(Constants.ButtonThemeText, x + Constants.ButtonWidthWithMargin, Game);
        Button exitButton = new(Constants.ButtonExitText, x + Constants.ButtonWidthWithMargin * 2, Game);

        // click handlers
        startButton.Click += StartButton_OnClick;
        themeButton.Click += ThemeButton_OnClick;
        exitButton.Click += ExitButton_OnClick;

        // save button references
        Buttons.AddRange(new Button[] { startButton, themeButton, exitButton });

        base.Initialize();
    }

    protected override void LoadContent()
    {
        Texture = Art.PokerTitle;
        base.LoadContent();
    }

    void StartButton_OnClick(object o, EventArgs e) =>
        ScreenManager.ShowScreen<GameplayScreen>();

    void ThemeButton_OnClick(object o, EventArgs e) =>
        ScreenManager.ShowScreen<ThemeScreen>();

    void ExitButton_OnClick(object o, EventArgs e) =>
        Game.Exit();
}