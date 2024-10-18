using Microsoft.Xna.Framework.Graphics;
using Solitaire.Managers;

namespace Solitaire.UI.BaseScreens;

abstract internal class GameScreen : DrawableGameComponent
{
    public static readonly int TopMargin = 5;
    public static readonly int BottomMargin = 30;
    public static readonly int HorizontalMargin = 30;
    static readonly Color BackgroundColor = new(50, 110, 51);
    public GameManager GameManager { get; }

    public GameScreen(GameManager gm) : base(gm.Game)
    {
        GameManager = gm;
        Hide();
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

    // draws green background with pattern
    public override void Draw(GameTime gameTime)
    {
        var sb = Game.Services.GetService<SpriteBatch>();
        Game.GraphicsDevice.Clear(BackgroundColor);
        sb.Begin();

        // draw tiles
        int xTileCount = SolitaireGame.Width / Art.BackgroundTile.Width;
        int yTileCount = SolitaireGame.Height / Art.BackgroundTile.Height;
        for (int y = 0; y < yTileCount; y++)
        {
            for (int x = 0; x < xTileCount; x++)
            {
                int u = x * Art.BackgroundTile.Width;
                int v = y * Art.BackgroundTile.Height;
                var dest = new Rectangle(u, v, Art.BackgroundTile.Width, Art.BackgroundTile.Height); 
                sb.Draw(Art.BackgroundTile, dest, Color.White);
            }
        }

        sb.End();
        base.Draw(gameTime);
    }

    public static Vector2 GetCenteredPosition(int width, int height)
    {
        int x = (SolitaireGame.Width - width) / 2;
        int y = (SolitaireGame.Height - height) / 2;
        return new Vector2(x, y);
    }

    public static Vector2 GetCenteredPosition(Rectangle rectangle) =>
        GetCenteredPosition(rectangle.Width, rectangle.Height);
}