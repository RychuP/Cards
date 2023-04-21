using Microsoft.Xna.Framework;
using Poker.UI;

namespace Poker.Screens;

internal class MainMenuScreen : GameScreen
{
    Button _startButton;
    Button _themeButton;
    Button _exitButton;

    public MainMenuScreen() : base("title")
    { }

    public override void LoadContent()
    {
        base.LoadContent();

        // calculate button positions
        int buttonCount = 3;
        int x = (Destination.Width - buttonCount * Button.Width - (buttonCount - 1) * ButtonSpacer) / 2;

        // create buttons
        _startButton = new("Play", x, ButtonRow, ScreenManager.Game);
        _themeButton = new("Theme", x + Button.Width + ButtonSpacer, ButtonRow, ScreenManager.Game);
        _exitButton = new("Exit", x + (Button.Width + ButtonSpacer) * 2, ButtonRow, ScreenManager.Game);

        // add buttons to game components
        ScreenManager.Game.Components.Add(_startButton);
        ScreenManager.Game.Components.Add(_themeButton);
        ScreenManager.Game.Components.Add(_exitButton);

        // show the game screen
        Visible = true;
        Enabled = true;
    }

    protected override void OnVisibleChanged()
    {
        base.OnVisibleChanged();
        _startButton.Visible = Visible;
        _themeButton.Visible = Visible;
        _exitButton.Visible = Visible;
    }

    protected override void OnEnabledChanged()
    {
        base.OnEnabledChanged();
        _startButton.Enabled = Enabled;
        _themeButton.Enabled = Enabled;
        _exitButton.Enabled = Enabled;
    }
}