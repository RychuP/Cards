using Microsoft.Xna.Framework;
using System;

namespace Poker.Screens;

internal class MenuEntry
{
    /// <summary>
    /// Text of this <see cref="MenuEntry"/>.
    /// </summary>
    public string Text { get; set; }

    /// <summary>
    /// Position at which to draw this <see cref="MenuEntry"/>.
    /// </summary>
    public Rectangle Destination { get; set; }

    public MenuEntry(string text)
    {
        Text = text;
    }

    void OnSelected()
    {
        Selected?.Invoke(this, EventArgs.Empty);
    }

    public event EventHandler Selected;
}