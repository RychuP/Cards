using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Poker.Screens;

internal class GameScreen
{
    bool _visible = false;
    bool _enabled = false;

    /// <summary>
    /// Y coordinate for buttons in pixels.
    /// </summary>
    protected const int ButtonRow = 632;

    /// <summary>
    /// Space between buttons in pixels.
    /// </summary>
    protected const int ButtonSpacer = 80;

    // background texture for the display
    Texture2D _texture;

    // texture file name to load
    readonly string _textureName;

    /// <summary>
    /// Texture destination for drawing.
    /// </summary>
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
    /// <see cref="ScreenManager"/> instance this <see cref="GameScreen"/> belongs to.
    /// </summary>
    public ScreenManager ScreenManager { get; set; }

    // constructor
    public GameScreen(string textureName)
    {
        _textureName = textureName;
        Destination = PokerGame.Area;
    }

    /// <summary>
    /// <see cref="ScreenManager"/> is available at this stage.
    /// </summary>
    public virtual void Initialize()
    { }

    public virtual void LoadContent()
    {
        _texture = ScreenManager.Game.Content.Load<Texture2D>($"Images/{_textureName}");
    }

    public virtual void UnloadContent() { }

    /// <summary>
    /// Sets <see cref="Visible"/> and <see cref="Enabled"/> to true.
    /// </summary>
    public virtual void Show()
    {
        Visible = true;
        Enabled = true;
    }

    /// <summary>
    /// Sets <see cref="Visible"/> and <see cref="Enabled"/> to false.
    /// </summary>
    public virtual void Hide()
    {
        Visible = false;
        Enabled = false;
    }

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