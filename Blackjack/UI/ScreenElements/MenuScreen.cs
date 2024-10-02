using System;
using System.Collections.Generic;
using Blackjack.Misc;
using Framework.Assets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Blackjack.UI.ScreenElements;

/// <summary>
/// Base class for screens that contain a menu of options. The user can
/// move up and down to select an entry, or cancel to back out of the screen.
/// </summary>
abstract class MenuScreen : GameScreen
{
    // the number of pixels to pad above and below menu entries for touch input
    const int MenuEntryPadding = 35;
    readonly List<MenuEntry> _menuEntries = new();
    int _selectedEntry = 0;
    readonly string _menuTitle;
    bool _isMouseDown = false;

    /// <summary>
    /// Gets the list of menu entries, so derived classes can add
    /// or change the menu contents.
    /// </summary>
    protected IList<MenuEntry> MenuEntries =>
        _menuEntries;

    /// <summary>
    /// Constructor.
    /// </summary>
    public MenuScreen(string menuTitle)
    {
        _menuTitle = menuTitle;
        TransitionOnTime = TimeSpan.FromSeconds(0.5);
        TransitionOffTime = TimeSpan.FromSeconds(0.5);
    }

    /// <summary>
    /// Allows the screen to create the hit bounds for a particular menu entry.
    /// </summary>
    protected virtual Rectangle GetMenuEntryHitBounds(MenuEntry entry)
    {
        // the hit bounds are the entire width of the screen, and the height of the entry
        // with some additional padding above and below.
        return new Rectangle(
            0,
            entry.Destination.Y - MenuEntryPadding,
            ScreenManager.GraphicsDevice.Viewport.Width,
            entry.GetHeight(this) + MenuEntryPadding * 2);
    }

    /// <summary>
    /// Responds to user input, changing the selected entry and accepting
    /// or cancelling the menu.
    /// </summary>
    public override void HandleInput(InputState input)
    {
        // we cancel the current menu screen if the user presses the back button
        if (input.IsNewButtonPress(Buttons.Back, ControllingPlayer, out PlayerIndex player))
            OnCancel(player);

        // Take care of Keyboard input
        if (input.IsMenuUp(ControllingPlayer))
        {
            _selectedEntry--;

            if (_selectedEntry < 0)
                _selectedEntry = _menuEntries.Count - 1;
        }
        else if (input.IsMenuDown(ControllingPlayer))
        {
            _selectedEntry++;

            if (_selectedEntry >= _menuEntries.Count)
                _selectedEntry = 0;
        }
        else if (input.IsNewKeyPress(Keys.Enter, ControllingPlayer, out player) ||
            input.IsNewKeyPress(Keys.Space, ControllingPlayer, out player))
        {
            OnSelectEntry(_selectedEntry, player);
        }

        MouseState state = Mouse.GetState();
        if (state.LeftButton == ButtonState.Released)
        {
            if (_isMouseDown)
            {
                _isMouseDown = false;
                // convert the position to a Point that we can test against a Rectangle
                Point clickLocation = new Point(state.X, state.Y);

                // iterate the entries to see if any were tapped
                for (int i = 0; i < _menuEntries.Count; i++)
                {
                    MenuEntry menuEntry = _menuEntries[i];

                    if (menuEntry.Destination.Contains(clickLocation))
                    {
                        // Select the entry. since gestures are only available on Windows Phone,
                        // we can safely pass PlayerIndex.One to all entries since there is only

                        // one player on Windows Phone.
                        OnSelectEntry(i, PlayerIndex.One);
                    }
                }
            }
        }
        else if (state.LeftButton == ButtonState.Pressed)
        {
            _isMouseDown = true;

            // convert the position to a Point that we can test against a Rectangle
            Point clickLocation = new Point(state.X, state.Y);

            // iterate the entries to see if any were tapped
            for (int i = 0; i < _menuEntries.Count; i++)
            {
                MenuEntry menuEntry = _menuEntries[i];

                if (menuEntry.Destination.Contains(clickLocation))
                    _selectedEntry = i;
            }
        }
    }

    /// <summary>
    /// Handler for when the user has chosen a menu entry.
    /// </summary>
    protected virtual void OnSelectEntry(int entryIndex, PlayerIndex playerIndex)
    {
        _menuEntries[entryIndex].OnSelected(playerIndex);
    }

    /// <summary>
    /// Handler for when the user has cancelled the menu.
    /// </summary>
    protected virtual void OnCancel(PlayerIndex playerIndex)
    {
        ExitScreen();
    }

    /// <summary>
    /// Helper overload makes it easy to use OnCancel as a MenuEntry event handler.
    /// </summary>
    protected void ExitButtton_OnSelected(object sender, PlayerIndexEventArgs e)
    {
        OnCancel(e.PlayerIndex);
    }

    /// <summary>
    /// Allows the screen the chance to position the menu entries. By default
    /// all menu entries are lined up in a vertical list, centered on the screen.
    /// </summary>
    protected virtual void UpdateMenuEntryLocations()
    {
        // Make the menu slide into place during transitions, using a
        // power curve to make things look more interesting (this makes
        // the movement slow down as it nears the end).
        float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

        // start at Y = 475; each X value is generated per entry
        Vector2 position = new(0f,
            ScreenManager.Game.Window.ClientBounds.Height / 2 -
            (_menuEntries[0].GetHeight(this) + MenuEntryPadding * 2 * _menuEntries.Count));

        // update each menu entry's location in turn
        for (int i = 0; i < _menuEntries.Count; i++)
        {
            MenuEntry menuEntry = _menuEntries[i];

            // each entry is to be centered horizontally
            position.X = ScreenManager.GraphicsDevice.Viewport.Width / 2 - menuEntry.GetWidth(this) / 2;

            if (ScreenState == ScreenState.TransitionOn)
                position.X -= transitionOffset * 256;
            else
                position.X += transitionOffset * 512;

            // set the entry's position
            //menuEntry.Position = position;

            // move down for the next entry the size of this entry plus our padding
            position.Y += menuEntry.GetHeight(this) + MenuEntryPadding * 2;
        }
    }

    /// <summary>
    /// Updates the menu.
    /// </summary>
    public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
    {
        base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

        // Update each nested MenuEntry object.
        for (int i = 0; i < _menuEntries.Count; i++)
        {
            bool isSelected = IsActive && i == _selectedEntry;
            UpdateMenuEntryDestination();
            _menuEntries[i].Update(this, isSelected, gameTime);
        }
    }

    /// <summary>
    /// Draws the menu.
    /// </summary>
    public override void Draw(GameTime gameTime)
    {
        // make sure our entries are in the right place before we draw them
        //UpdateMenuEntryLocations();

        GraphicsDevice graphics = ScreenManager.GraphicsDevice;
        var sb = ScreenManager.Game.Services.GetService(typeof(SpriteBatch)) as SpriteBatch;

        sb.Begin();

        // Draw each menu entry in turn.
        for (int i = 0; i < _menuEntries.Count; i++)
        {
            MenuEntry menuEntry = _menuEntries[i];
            bool isSelected = IsActive && i == _selectedEntry;
            menuEntry.Draw(this, isSelected, gameTime);
        }

        // Make the menu slide into place during transitions, using a
        // power curve to make things look more interesting (this makes
        // the movement slow down as it nears the end).
        float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

        // Draw the menu title centered on the screen
        Vector2 titlePosition = new Vector2(graphics.Viewport.Width / 2, 375);
        Vector2 titleOrigin = Fonts.Moire.Menu.MeasureString(_menuTitle) / 2;
        Color titleColor = new Color(192, 192, 192) * TransitionAlpha;
        float titleScale = 1.25f;

        titlePosition.Y -= transitionOffset * 100;

        sb.DrawString(Fonts.Moire.Menu, _menuTitle, titlePosition, titleColor, 0,
                               titleOrigin, titleScale, SpriteEffects.None, 0);

        sb.End();
    }

    public void UpdateMenuEntryDestination()
    {
        Rectangle bounds = ScreenManager.SafeArea;

        Rectangle textureSize = Art.Button.Bounds;
        int xStep = bounds.Width / (_menuEntries.Count + 2);
        int maxWidth = 0;

        for (int i = 0; i < _menuEntries.Count; i++)
        {
            int width = _menuEntries[i].GetWidth(this);
            if (width > maxWidth)
                maxWidth = width;
        }
        maxWidth += 20;

        for (int i = 0; i < _menuEntries.Count; i++)
        {
            _menuEntries[i].Destination = new Rectangle(
                bounds.Left + (xStep - textureSize.Width) / 2 + (i + 1) * xStep,
                bounds.Bottom - textureSize.Height * 2,
                maxWidth, 50
            );
        }
    }
}