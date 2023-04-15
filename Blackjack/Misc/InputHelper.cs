#region File Description
//-----------------------------------------------------------------------------
// InputHelper.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Blackjack;

/// <summary>
/// Used to simulate a cursor.
/// </summary>
public class InputHelper : DrawableGameComponent
{
    #region Fields
    public bool IsEscape { get; set; }
    public bool IsPressed { get; set; }

    Vector2 _drawPosition;
    readonly Texture2D _texture;
    readonly SpriteBatch _spriteBatch;
    readonly float _maxVelocity;
    #endregion

    #region Initialization
    public InputHelper(Game game) : base(game)
    {
        _texture = Game.Content.Load<Texture2D>(@"Images\GamePadCursor");
        _spriteBatch = new SpriteBatch(Game.GraphicsDevice);

        _maxVelocity = (Game.GraphicsDevice.Viewport.Width +
            Game.GraphicsDevice.Viewport.Height) / 3000f;

        _drawPosition = new Vector2(Game.GraphicsDevice.Viewport.Width / 2,
            Game.GraphicsDevice.Viewport.Height / 2);
    }
    #endregion

    #region Properties
    public Vector2 PointPosition =>
        _drawPosition + new Vector2(_texture.Width / 2f, _texture.Height / 2f);
    #endregion

    #region Update and Render
    /// <summary>
    /// Updates itself.
    /// </summary>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    public override void Update(GameTime gameTime)
    {
        GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);

        IsPressed = gamePadState.Buttons.A == ButtonState.Pressed;

        IsEscape = gamePadState.Buttons.Back == ButtonState.Pressed;

        _drawPosition += gamePadState.ThumbSticks.Left
            * new Vector2(1, -1)
            * gameTime.ElapsedGameTime.Milliseconds
            * _maxVelocity;
        _drawPosition = Vector2.Clamp(_drawPosition, Vector2.Zero,
            new Vector2(Game.GraphicsDevice.Viewport.Width, Game.GraphicsDevice.Viewport.Height)
            - new Vector2(_texture.Bounds.Width, _texture.Bounds.Height));
    }

    /// <summary>
    /// Draws cursor.
    /// </summary>
    public override void Draw(GameTime gameTime)
    {
        _spriteBatch.Begin();
        _spriteBatch.Draw(_texture, _drawPosition, null, Color.White, 0, Vector2.Zero, 1,
            SpriteEffects.None, 0);
        _spriteBatch.End();
    }
    #endregion
}