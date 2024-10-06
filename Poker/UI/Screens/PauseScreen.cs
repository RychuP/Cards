using Poker.Gameplay;
using Poker.UI.BaseScreens;
using System;

namespace Poker.UI.Screens;

class PauseScreen : StaticGameScreen
{
    public PauseScreen(ScreenManager screenManager) : base(screenManager, 2)
    {
        AddButton("Continue", ContinueButton_OnClick);
        AddButton("Exit", ExitButton_OnClick);
        DrawOrder = int.MaxValue;
    }

    void ContinueButton_OnClick(object o, EventArgs e) =>
        Game.Services.GetService<GameManager>().ResumeGame();

    void ExitButton_OnClick(object o, EventArgs e) =>
        Game.Services.GetService<GameManager>().StopPlaying();

    protected override void LoadContent()
    {
        Texture = Art.PauseTitle;
        base.LoadContent();
    }
}