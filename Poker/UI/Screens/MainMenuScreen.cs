using System;
using Poker.Gameplay;
using Poker.UI.BaseGameScreens;

namespace Poker.UI.Screens;

/// <summary>
/// Main menu screen shown before the game starts.
/// </summary>
class MainMenuScreen : StaticGameScreen
{
    public MainMenuScreen(ScreenManager screenManager) : base(screenManager, 3)
    { }

    public override void Initialize()
    {
        // calculate left most button position
        int x = (Constants.GameWidth - Buttons.Capacity * Constants.ButtonWidthWithPadding
            + Constants.ButtonPadding) / 2;

        // create buttons
        Button startButton = new(Constants.ButtonPlayText, x, Game);
        Button themeButton = new(Constants.ButtonThemeText, x + Constants.ButtonWidthWithPadding, Game);
        Button exitButton = new(Constants.ButtonExitText, x + Constants.ButtonWidthWithPadding * 2, Game);

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

    void StartButton_OnClick(object o, EventArgs e)
    {
        ScreenManager.ShowScreen<GameplayScreen>();
        Game.Services.GetService<GameManager>().StartPlaying();
    }

    void ThemeButton_OnClick(object o, EventArgs e) =>
        ScreenManager.ShowScreen<ThemeScreen>();

    void ExitButton_OnClick(object o, EventArgs e) =>
        Game.Exit();
}