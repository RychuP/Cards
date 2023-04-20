using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Poker.Screens;

internal class BackgroundScreen : GameScreen
{
    Texture2D _background;
    Rectangle _destination;

    public BackgroundScreen()
    {
        Visible = true;
    }

    public override void Initialize()
    {
        _destination = ScreenManager.GraphicsDevice.Viewport.Bounds;
    }

    public override void LoadContent()
    {
        _background = ScreenManager.Game.Content.Load<Texture2D>("Images/background");
        
        base.LoadContent();
    }

    public override void Draw(GameTime gameTime)
    {
        ScreenManager.SpriteBatch.Begin();
        ScreenManager.SpriteBatch.Draw(_background, _destination, Color.White);
        ScreenManager.SpriteBatch.End();
    }
}