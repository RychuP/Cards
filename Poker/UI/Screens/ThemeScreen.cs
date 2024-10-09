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
        AddButton("Red", RedThemeButton_OnClick);
        AddButton("Blue", BlueThemeButton_OnClick);
        AddButton("Exit", ExitButton_OnClick);

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
        if (e.PrevScreen != this && e.NewScreen != this)
            return;

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

    void ExitButton_OnClick(object o, EventArgs e) =>
        ScreenManager.ShowScreen<StartScreen>();
}