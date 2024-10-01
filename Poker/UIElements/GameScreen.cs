using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Poker.Misc;

namespace Poker.UIElements;

abstract class GameScreen : DrawableGameComponent
{
    protected Texture2D Texture { get; set; }

    protected ScreenManager ScreenManager { get; }

    public GameScreen(ScreenManager screenManager) : base(screenManager.Game)
    {
        ScreenManager = screenManager;
    }

    public override void Initialize()
    {
        Hide();
        base.Initialize();
    }

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

    public override void Draw(GameTime gameTime)
    {
        Art.SpriteBatch.Begin();
        Art.SpriteBatch.Draw(Texture, Constants.GameArea, Color.White);
        Art.SpriteBatch.End();
        base.Draw(gameTime);
    }
}