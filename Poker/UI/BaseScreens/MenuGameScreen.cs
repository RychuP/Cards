using System.Collections.Generic;
using Poker.UI.AnimatedGameComponents;

namespace Poker.UI.BaseScreens;

/// <summary>
/// Base class for menu screens with buttons.
/// </summary>
/// <remarks>Buttons occupy one line below the table and share the same Y.</remarks>
abstract class MenuGameScreen : GameScreen
{
    /// <summary>
    /// List of buttons in this menu screen.
    /// </summary>
    protected List<Button> Buttons { get; init; }

    public MenuGameScreen(ScreenManager screenManager, int buttonCount) : base(screenManager)
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