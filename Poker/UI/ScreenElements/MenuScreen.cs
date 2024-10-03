using System.Collections.Generic;

namespace Poker.UI.ScreenElements;

/// <summary>
/// Base class for menu screens with buttons.
/// </summary>
abstract class MenuScreen : GameScreen
{
    /// <summary>
    /// List of buttons in this menu screen.
    /// </summary>
    protected List<Button> Buttons { get; init; }

    public MenuScreen(ScreenManager screenManager, int buttonCount) : base(screenManager)
    {
        Buttons = new(buttonCount);
    }

    public override void Initialize()
    {
        // add buttons to game components
        foreach (var button in Buttons)
            Game.Components.Add(button);

        base.Initialize();
    }

    public override void Hide()
    {
        base.Hide();
        foreach (var button in Buttons)
            button.Hide();
    }
}