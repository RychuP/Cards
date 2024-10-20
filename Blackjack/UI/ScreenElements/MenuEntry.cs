using System;
using Blackjack.Misc;
using Framework.Assets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Blackjack.UI.ScreenElements;

/// <summary>
/// Helper class represents a single entry in a MenuScreen. By default this
/// just draws the entry text string, but it can be customized to display menu
/// entries in different ways. This also provides an event that will be raised
/// when the menu entry is selected.
/// </summary>
class MenuEntry
{
    /// <summary>
    /// Tracks a fading selection effect on the entry.
    /// </summary>
    /// <remarks>
    /// The entries transition out of the selection effect when they are deselected.
    /// </remarks>
    float _selectionFade;

    /// <summary>
    /// Gets or sets the text of this menu entry.
    /// </summary>
    public string Text { get; set; }

    /// <summary>
    /// Gets or sets the position at which to draw this menu entry.
    /// </summary>
    public Rectangle Destination { get; set; }

    public float Scale { get; set; } = 1f;

    public float Rotation { get; set; } = 0f;

    /// <summary>
    /// Event raised when the menu entry is selected.
    /// </summary>
    public event EventHandler<PlayerIndexEventArgs> Selected;

    /// <summary>
    /// Method for raising the Selected event.
    /// </summary>
    protected internal virtual void OnSelected(PlayerIndex playerIndex)
    {
        Selected?.Invoke(this, new PlayerIndexEventArgs(playerIndex));
    }

    /// <summary>
    /// Constructs a new menu entry with the specified text.
    /// </summary>
    public MenuEntry(string text)
    {
        Text = text;
    }

    /// <summary>
    /// Updates the menu entry.
    /// </summary>
    public virtual void Update(MenuScreen screen, bool isSelected, GameTime gameTime)
    {
        // When the menu selection changes, entries gradually fade between
        // their selected and deselected appearance, rather than instantly
        // popping to the new state.
        float fadeSpeed = (float)gameTime.ElapsedGameTime.TotalSeconds * 4;

        if (isSelected)
            _selectionFade = Math.Min(_selectionFade + fadeSpeed, 1);
        else
            _selectionFade = Math.Max(_selectionFade - fadeSpeed, 0);
    }

    /// <summary>
    /// Draws the menu entry. This can be overridden to customize the appearance.
    /// </summary>
    public virtual void Draw(MenuScreen screen, bool isSelected, GameTime gameTime)
    {
        Color textColor = isSelected ? Color.White : Color.Black;
        Color tintColor = isSelected ? Color.White : Color.Gray;

        // Draw text, centered on the middle of each line.
        ScreenManager screenManager = screen.ScreenManager;
        var sb = screenManager.Game.Services.GetService<SpriteBatch>();

        sb.Draw(Art.Button, Destination, tintColor);

        sb.DrawString(Fonts.Moire.Menu, Text, GetTextPosition(screen),
            textColor, Rotation, Vector2.Zero, Scale, SpriteEffects.None, 0);
    }

    /// <summary>
    /// Queries how much space this menu entry requires.
    /// </summary>
    public virtual int GetHeight(MenuScreen screen) =>
        Fonts.Moire.Menu.LineSpacing;

    /// <summary>
    /// Queries how wide the entry is, used for centering on the screen.
    /// </summary>
    public virtual int GetWidth(MenuScreen screen) =>
        (int)Fonts.Moire.Menu.MeasureString(Text).X;

    Vector2 GetTextPosition(MenuScreen screen)
    {
        int x = Destination.X + (Destination.Width - GetWidth(screen)) / 2;
        int y = Destination.Y;
        return Scale == 1f ? new Vector2(x, y) :
            new Vector2(x * Scale, y + (GetHeight(screen) - GetHeight(screen) * Scale) / 2);
    }
}