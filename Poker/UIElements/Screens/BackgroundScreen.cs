using Poker.Misc;

namespace Poker.UIElements.Screens;

/// <summary>
/// Static background screen shown everywhere.
/// </summary>
class BackgroundScreen : GameScreen
{
    public BackgroundScreen(ScreenManager screenManager) : base(screenManager)
    { }

    public override void Initialize()
    {
        base.Initialize();
        Visible = true;
    }

    protected override void LoadContent()
    {
        base.LoadContent();
        Texture = Art.Table;
    }
}