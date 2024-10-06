using System;
using Poker.UI.AnimatedGameComponents;

namespace Poker.UI.BaseScreens;

/// <summary>
/// Menu screen with static buttons that always show.
/// </summary>
abstract class StaticGameScreen : MenuGameScreen
{
    // x coordinate of the left most button on the screen
    readonly int _leftMostButtonX;

    public StaticGameScreen(ScreenManager screenManager, int buttonCount) : base(screenManager, buttonCount)
    {
        // calculate left most button position
        _leftMostButtonX = (Constants.GameWidth - Buttons.Capacity * Constants.ButtonWidthWithPadding
            + Constants.ButtonPadding) / 2;
    }

    protected void AddButton(string text, EventHandler handler)
    {
        if (Buttons.Count == Buttons.Capacity) 
            throw new ArgumentException("Cannot add more buttons. The list is full.");
        int nextButtonIndex = Buttons.Count;
        int x = _leftMostButtonX + Constants.ButtonWidthWithPadding * nextButtonIndex;
        Button button = new(text, x, Game);
        button.Click += handler;
        Buttons.Add(button);
    }

    public override void Show()
    {
        base.Show();
        foreach (var button in Buttons)
            button.Show();
    }
}