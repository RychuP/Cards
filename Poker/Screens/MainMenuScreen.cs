using Poker.UI;

namespace Poker.Screens;

/// <summary>
/// Main menu screen shown when the game loads.
/// </summary>
internal class MainMenuScreen : MenuScreen
{
    public MainMenuScreen() : base("title", 3)
    { }

    public override void LoadContent()
    {
        // calculate button positions
        int buttonCount = 3;
        int x = (Destination.Width - buttonCount * Button.Width - (buttonCount - 1) * ButtonSpacer) / 2;

        // create buttons
        Button start = new("Play", x, ButtonRow, ScreenManager.Game);
        Button theme = new("Theme", x + Button.Width + ButtonSpacer, ButtonRow, ScreenManager.Game);
        Button exit = new("Exit", x + (Button.Width + ButtonSpacer) * 2, ButtonRow, ScreenManager.Game);

        // click handlers
        exit.Click += (o, e) => ScreenManager.Game.Exit();

        // save button references
        Buttons.AddRange(new Button[] {start, theme, exit});

        // show content
        Visible = true;
        Enabled = true;

        // add buttons to components andd load texture
        base.LoadContent();
    }
}