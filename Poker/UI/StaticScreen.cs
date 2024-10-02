namespace Poker.UI;

/// <summary>
/// Menu screen with static buttons that always show.
/// </summary>
abstract class StaticScreen : MenuScreen
{
    public StaticScreen(ScreenManager screenManager, int buttonCount) : base(screenManager, buttonCount)
    { }

    public override void Show()
    {
        base.Show();
        foreach (var button in Buttons)
            button.Show();
    }
}