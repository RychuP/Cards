using Microsoft.Xna.Framework;
using Poker.UI;
using System;
using System.Collections.Generic;

namespace Poker.Screens;

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

    protected override void OnVisibleChanged(object sender, EventArgs args)
    {
        base.OnVisibleChanged(sender, args);

        // show buttons
        if (!Visible)
        {
            foreach (var button in Buttons)
                button.Visible = Visible;
        }
    }

    protected override void OnEnabledChanged(object sender, EventArgs args)
    {
        base.OnEnabledChanged(sender, args);

        // hide buttons
        if (!Enabled)
        {
            foreach (var button in Buttons)
                button.Enabled = Enabled;
        }
    }
}