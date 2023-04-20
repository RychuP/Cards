using Microsoft.Xna.Framework;

namespace Poker.Screens;

internal class GameScreen
{
    /// <summary>
    /// Whether this screen should process draw.
    /// </summary>
    public bool Visible { get; set; } = false;

    /// <summary>
    /// Whether this screen should process update.
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Whether this screen should process input.
    /// </summary>
    public bool Focused { get; set; } = false;

    /// <summary>
    /// <see cref="ScreenManager"/> instance this <see cref="GameScreen"/> belongs to.
    /// </summary>
    public ScreenManager ScreenManager { get; set; }

    /// <summary>
    /// <see cref="ScreenManager"/> is available at this stage.
    /// </summary>
    public virtual void Initialize() { }

    public virtual void LoadContent() { }

    public virtual void UnloadContent() { }

    public virtual void Update(GameTime gameTime) { }

    public virtual void Draw(GameTime gameTime) { }
}