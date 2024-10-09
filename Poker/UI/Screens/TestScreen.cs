using Poker.UI.BaseScreens;
using System;

namespace Poker.UI.Screens;

internal class TestScreen : StaticGameScreen
{
    public TestScreen(ScreenManager sm) : base(sm, 2)
    { }

    public override void Initialize()
    {
        AddButton("Shuffle", ShuffleButton_OnClick);
        AddButton("Exit", ExitButton_OnClick);

        base.Initialize();
    }

    protected override void LoadContent()
    {
        Texture = Art.TableCardOutlines;
        base.LoadContent();
    }

    void ShuffleButton_OnClick(object o, EventArgs e)
    {

    }

    void ExitButton_OnClick(object o, EventArgs e) =>
        ScreenManager.ShowScreen<StartScreen>();
}