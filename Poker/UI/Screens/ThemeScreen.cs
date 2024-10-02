using Poker.UI;
using System;

namespace Poker.UIElements.Screens;

class ThemeScreen : StaticScreen
{
    public ThemeScreen(ScreenManager screenManager) : base(screenManager, 3)
    { }

    public override void Initialize()
    {
        // calculate left most button position
        int x = (Constants.GameWidth - Buttons.Capacity * Constants.ButtonWidthWithMargin
            + Constants.SpaceBetweenButtons) / 2;

        // create buttons
        Button redThemeButton = new(Constants.RedThemeText, x, Game);
        Button blueThemeButton = new(Constants.BlueThemeText, x + Constants.ButtonWidthWithMargin, Game);
        Button returnButton = new(Constants.ButtonReturnText, x + Constants.ButtonWidthWithMargin * 2, Game);

        // click handlers
        redThemeButton.Click += RedThemeButton_OnClick;
        blueThemeButton.Click += BlueThemeButton_OnClick;
        returnButton.Click += ReturnButton_OnClick;

        // save button references
        Buttons.AddRange(new Button[] {redThemeButton, blueThemeButton, returnButton});

        base.Initialize();
    }

    protected override void LoadContent()
    {
        Texture = Art.ThemeTitle;
        base.LoadContent();
    }

    void RedThemeButton_OnClick(object o, EventArgs e) =>
        ((PokerGame)Game).GameManager.SetTheme(Constants.RedThemeText);

    void BlueThemeButton_OnClick(object o, EventArgs e) =>
        ((PokerGame)Game).GameManager.SetTheme(Constants.BlueThemeText);

    void ReturnButton_OnClick(object o, EventArgs e) =>
        ScreenManager.ShowScreen<MainMenuScreen>();
}