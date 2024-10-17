using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Framework.Engine;
using Framework.UI;
using Framework.Assets;
using Blackjack.Misc;

namespace Blackjack.UI.Components;

public class Button : AnimatedGameComponent
{
    bool _isKeyDown = false;
    bool _isPressed = false;
    public event EventHandler Click;
    public Rectangle Bounds { get; set; }

    /// <summary>
    /// Creates a new instance of the <see cref="Button"/> class.
    /// </summary>
    /// <param name="cardGame">The associated card game.</param>
    public Button(CardGame cardGame) : base(cardGame, null)
    { }

    /// <summary>
    /// Performs update logic for the button.
    /// </summary>
    /// <param name="gameTime">The time that has passed since the last call to 
    /// this method.</param>
    public override void Update(GameTime gameTime)
    {
        HandleInput(Mouse.GetState());
        base.Update(gameTime);
    }

    /// <summary>
    /// Handle the input of adding chip on all platform
    /// </summary>
    /// <param name="mouseState">Mouse input information.</param>
    /// <param name="inputHelper">Input of Xbox simulated cursor.</param>
    private void HandleInput(MouseState mouseState)
    {
        bool pressed = false;
        Vector2 position = Vector2.Zero;

        if (mouseState.LeftButton == ButtonState.Pressed)
        {
            pressed = true;
            position = new Vector2(mouseState.X, mouseState.Y);
        }
        else
        {
            if (_isPressed)
            {
                if (IntersectWith(new Vector2(mouseState.X, mouseState.Y)))
                {
                    OnClick();
                    _isPressed = false;
                }
                else
                {
                    _isPressed = false;
                }
            }

            _isKeyDown = false;
        }

        if (pressed)
        {
            if (!_isKeyDown)
            {
                if (IntersectWith(position))
                {
                    _isPressed = true;
                }
                _isKeyDown = true;
            }
        }
        else
        {
            _isKeyDown = false;
        }
    }

    /// <summary>
    /// Checks if the button intersects with a specified position
    /// </summary>
    /// <param name="position">The position to check intersection against.</param>
    /// <returns>True if the position intersects with the button, 
    /// false otherwise.</returns>
    private bool IntersectWith(Vector2 position)
    {
        Rectangle touchTap = new((int)position.X - 1, (int)position.Y - 1, 2, 2);
        return Bounds.Intersects(touchTap);
    }

    /// <summary>
    /// Fires the button's click event.
    /// </summary>
    public void OnClick()
    {
        Click?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Draws the button.
    /// </summary>
    /// <param name="gameTime">The time that has passed since the last call to 
    /// this method.</param>
    public override void Draw(GameTime gameTime)
    {
        var sb = Game.Services.GetService(typeof(SpriteBatch)) as SpriteBatch;
        sb.Begin();

        sb.Draw(_isPressed ? Art.ButtonPressed : Art.ButtonRegular, Bounds, Color.White);

        Vector2 textPosition = Fonts.Moire.Regular.MeasureString(Text);
        textPosition = new Vector2(Bounds.Width - textPosition.X,
            Bounds.Height - textPosition.Y);
        textPosition /= 2;
        textPosition.X += Bounds.X;
        textPosition.Y += Bounds.Y;
        sb.DrawString(Fonts.Moire.Regular, Text, textPosition, Color.White);

        sb.End();

        base.Draw(gameTime);
    }

    protected override void Dispose(bool disposing)
    {
        Click = null;
        base.Dispose(disposing);
    }
}