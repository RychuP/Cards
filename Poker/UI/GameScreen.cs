using Microsoft.Xna.Framework.Graphics;

namespace Poker.UI;

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
        var sb = Game.Services.GetService(typeof(SpriteBatch)) as SpriteBatch;
        sb.Begin();
        sb.Draw(Texture, Constants.GameArea, Color.White);
        sb.End();
        base.Draw(gameTime);
    }
}