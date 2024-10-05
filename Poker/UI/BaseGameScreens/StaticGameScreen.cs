namespace Poker.UI.BaseGameScreens;

/// <summary>
/// Menu screen with static buttons that always show.
/// </summary>
abstract class StaticGameScreen : ButtonGameScreen
{
    public StaticGameScreen(ScreenManager screenManager, int buttonCount) : base(screenManager, buttonCount)
    { }

    public override void Show()
    {
        base.Show();
        foreach (var button in Buttons)
            button.Show();
    }
}