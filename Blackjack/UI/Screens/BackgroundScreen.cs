using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Blackjack.UI.ScreenElements;
using Blackjack.Misc;

namespace Blackjack.UI.Screens;

class BackgroundScreen : GameScreen
{
    /// <summary>
    /// Initializes a new instance of the screen.
    /// </summary>
    public BackgroundScreen()
    {
        TransitionOnTime = TimeSpan.FromSeconds(0.0);
        TransitionOffTime = TimeSpan.FromSeconds(0.5);
    }

    /// <summary>
    /// Allows the screen to run logic, such as updating the transition position.
    /// Unlike HandleInput, this method is called regardless of whether the screen
    /// is active, hidden, or in the middle of a transition.
    /// </summary>
    /// <param name="gameTime"></param>
    /// <param name="otherScreenHasFocus"></param>
    /// <param name="coveredByOtherScreen"></param>
    public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
    {
        base.Update(gameTime, otherScreenHasFocus, false);
    }

    /// <summary>
    /// This is called when the screen should draw itself.
    /// </summary>
    /// <param name="gameTime"></param>
    public override void Draw(GameTime gameTime)
    {
        var sb = ScreenManager.Game.Services.GetService(typeof(SpriteBatch)) as SpriteBatch;
        sb.Begin();
        sb.Draw(Art.TitleScreen, ScreenManager.GraphicsDevice.Viewport.Bounds,
            Color.White * TransitionAlpha);
        sb.End();

        base.Draw(gameTime);
    }
}