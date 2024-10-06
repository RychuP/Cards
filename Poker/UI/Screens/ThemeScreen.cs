using Poker.Gameplay;
using Poker.UI.AnimatedGameComponents;
using Poker.UI.BaseScreens;
using System;

namespace Poker.UI.Screens;

class ThemeScreen : StaticGameScreen
{
    public ThemeScreen(ScreenManager screenManager) : base(screenManager, 3)
    { }

    public override void Initialize()
    {
        // calculate left most button position
        int x = (Constants.GameWidth - Buttons.Capacity * Constants.ButtonWidthWithPadding
            + Constants.ButtonPadding) / 2;

        // create buttons
        Button redThemeButton = new(Constants.RedThemeText, x, Game);
        Button blueThemeButton = new(Constants.BlueThemeText, x + Constants.ButtonWidthWithPadding, Game);
        Button returnButton = new(Constants.ButtonReturnText, x + Constants.ButtonWidthWithPadding * 2, Game);

        // click handlers
        redThemeButton.Click += RedThemeButton_OnClick;
        blueThemeButton.Click += BlueThemeButton_OnClick;
        returnButton.Click += ReturnButton_OnClick;

        // save button references
        Buttons.AddRange(new Button[] { redThemeButton, blueThemeButton, returnButton });

        // register event handlers
        ScreenManager.ScreenChanged += ScreenManager_ScreenChanged;

        base.Initialize();
    }

    protected override void LoadContent()
    {
        Texture = Art.ThemeTitle;
        base.LoadContent();
    }

    void ScreenManager_ScreenChanged(object o, ScreenChangedEventArgs e)
    {
        var cardPile = Game.Components.Find<AnimatedCardPile>();
        if (cardPile is not null)
        {
            if (e.NewScreen == this)
            {
                cardPile.Reset();
                cardPile.SlideDown();
            }
            else if (e.PrevScreen == this)
                cardPile.SlideUp();
        }
    }

    void RedThemeButton_OnClick(object o, EventArgs e) =>
        Game.Services.GetService<GameManager>().SetTheme(Constants.RedThemeText);

    void BlueThemeButton_OnClick(object o, EventArgs e) =>
        Game.Services.GetService<GameManager>().SetTheme(Constants.BlueThemeText);

    void ReturnButton_OnClick(object o, EventArgs e) =>
        ScreenManager.ShowScreen<StartScreen>();
}