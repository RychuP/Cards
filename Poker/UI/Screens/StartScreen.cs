using System;
using Poker.Gameplay;
using Poker.UI.BaseScreens;

namespace Poker.UI.Screens;

/// <summary>
/// Start screen shown when the game first loads.
/// </summary>
class StartScreen : StaticGameScreen
{
    public StartScreen(ScreenManager screenManager) : base(screenManager, 4)
    { }

    public override void Initialize()
    {
        AddButton("Play", StartButton_OnClick);
        AddButton("Theme", ThemeButton_OnClick);
        AddButton("Test", TestButton_OnClick);
        AddButton("Exit", ExitButton_OnClick);

        base.Initialize();
    }

    protected override void LoadContent()
    {
        Texture = Art.PokerTitle;
        base.LoadContent();
    }

    void StartButton_OnClick(object o, EventArgs e)
    {
        var gm = Game.Services.GetService<GameManager>();
        gm.Reset();
        gm.StartPlaying();
        ScreenManager.ShowScreen<GameplayScreen>();
    }

    void ThemeButton_OnClick(object o, EventArgs e) =>
        ScreenManager.ShowScreen<ThemeScreen>();

    void ExitButton_OnClick(object o, EventArgs e) =>
        Game.Exit();

    void TestButton_OnClick(object o, EventArgs e) =>
        ScreenManager.ShowScreen<TestScreen>();
}