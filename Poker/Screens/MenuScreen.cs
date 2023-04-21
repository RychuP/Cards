using Poker.UI;
using System.Collections.Generic;

namespace Poker.Screens;

/// <summary>
/// Base class for menu screens with buttons.
/// </summary>
internal class MenuScreen : GameScreen
{
    /// <summary>
    /// List of buttons in this menu screen.
    /// </summary>
    protected List<Button> Buttons { get; init; }

    public MenuScreen(string textureName, int buttonCount) : base(textureName)
    {
        Buttons = new(buttonCount);
    }

    protected override void OnVisibleChanged()
    {
        base.OnVisibleChanged();
        foreach (var button in Buttons)
            button.Visible = Visible;
    }

    protected override void OnEnabledChanged()
    {
        base.OnEnabledChanged();
        foreach (var button in Buttons)
            button.Enabled = Enabled;
    }

    public override void LoadContent()
    {
        // add buttons to game components
        foreach (var button in Buttons)
            ScreenManager.Game.Components.Add(button);

        // load texture
        base.LoadContent();
    }
}