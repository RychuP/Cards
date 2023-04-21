using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Poker.Screens;

internal class GameScreen
{
    bool _visible = false;
    bool _enabled = false;

    // y coordinate for buttons
    protected const int ButtonRow = 632;

    // space between buttons
    protected const int ButtonSpacer = 80;

    // background texture for the display
    Texture2D _texture;

    // texture file name to load
    readonly string _textureName;

    // texture destination
    protected Rectangle Destination { get; private set; }

    /// <summary>
    /// Whether this screen should process draw.
    /// </summary>
    public bool Visible
    {
        get => _visible;
        set
        {
            if (_visible == value) return;
            _visible = value;
            OnVisibleChanged();
        }
    }

    /// <summary>
    /// Whether this screen should process update.
    /// </summary>
    public bool Enabled
    {
        get => _enabled;
        set
        {
            if (_enabled == value) return;
            _enabled = value;
            OnEnabledChanged();
        }
    }

    /// <summary>
    /// Whether this screen should process input.
    /// </summary>
    public bool Focused { get; set; } = false;

    /// <summary>
    /// <see cref="ScreenManager"/> instance this <see cref="GameScreen"/> belongs to.
    /// </summary>
    public ScreenManager ScreenManager { get; set; }

    public GameScreen(string textureName) =>
        _textureName = textureName;

    /// <summary>
    /// <see cref="ScreenManager"/> is available at this stage.
    /// </summary>
    public virtual void Initialize()
    {
        Destination = ScreenManager.GraphicsDevice.Viewport.Bounds;
    }

    public virtual void LoadContent()
    {
        _texture = ScreenManager.Game.Content.Load<Texture2D>($"Images/{_textureName}");
    }

    public virtual void UnloadContent() { }

    public virtual void Update(GameTime gameTime) { }

    public virtual void Draw(GameTime gameTime)
    {
        ScreenManager.SpriteBatch.Begin();
        ScreenManager.SpriteBatch.Draw(_texture, Destination, Color.White);
        ScreenManager.SpriteBatch.End();
    }

    protected virtual void OnVisibleChanged()
    {
        VisibleChanged?.Invoke(this, EventArgs.Empty);
    }

    protected virtual void OnEnabledChanged()
    {
        EnabledChanged?.Invoke(this, EventArgs.Empty);
    }

    public event EventHandler VisibleChanged;
    public event EventHandler EnabledChanged;
}