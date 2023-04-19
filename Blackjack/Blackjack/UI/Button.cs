#region File Description
//-----------------------------------------------------------------------------
// Button.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using GameStateManagement;
using CardsFramework;

namespace Blackjack;

public class Button : AnimatedGameComponent
{
    #region Fields and Properties
    bool _isKeyDown = false;
    bool _isPressed = false;

    public Texture2D RegularTexture { get; set; }
    public Texture2D PressedTexture { get; set; }
    public SpriteFont Font { get; set; }
    public Rectangle Bounds { get; set; }

    readonly string _regularTexture;
    readonly string _pressedTexture;

    public event EventHandler Click;
    InputHelper inputHelper;

    #endregion

    #region Initializations
    /// <summary>
    /// Creates a new instance of the <see cref="Button"/> class.
    /// </summary>
    /// <param name="regularTexture">The name of the button's texture.</param>
    /// <param name="pressedTexture">The name of the texture to display when the 
    /// button is pressed.</param>
    /// <param name="input">A <see cref="InputState"/> object
    /// which can be used to retrieve user input.</param>
    /// <param name="cardGame">The associated card game.</param>
    /// <remarks>Texture names are relative to the "Images" content 
    /// folder.</remarks>
    public Button(string regularTexture, string pressedTexture, CardsGame cardGame)
        : base(cardGame, null)
    {
        _regularTexture = regularTexture;
        _pressedTexture = pressedTexture;
        Font = cardGame.Font;
    }

    /// <summary>
    /// Initializes the button.
    /// </summary>
    public override void Initialize()
    {
        // Get Xbox curser
        inputHelper = null;
        for (int componentIndex = 0; componentIndex < Game.Components.Count; componentIndex++)
        {
            if (Game.Components[componentIndex] is InputHelper helper)
            {
                inputHelper = helper;
                break;
            }
        }

        base.Initialize();
    }
    #endregion

    #region Loading
    /// <summary>
    /// Loads the content required bt the button.
    /// </summary>
    protected override void LoadContent()
    {
        if (_regularTexture != null)
        {
            RegularTexture = Game.Content.Load<Texture2D>(@"Images\" + _regularTexture);
        }
        if (_pressedTexture != null)
        {
            PressedTexture = Game.Content.Load<Texture2D>(@"Images\" + _pressedTexture);
        }
        base.LoadContent();
    }
    #endregion

    #region Update and Render
    /// <summary>
    /// Performs update logic for the button.
    /// </summary>
    /// <param name="gameTime">The time that has passed since the last call to 
    /// this method.</param>
    public override void Update(GameTime gameTime)
    {
        if (RegularTexture != null)
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
        else if (inputHelper is not null && inputHelper.IsPressed)
        {
            pressed = true;
            position = inputHelper.PointPosition;
        }
        else
        {
            if (_isPressed)
            {
                if (IntersectWith(new Vector2(mouseState.X, mouseState.Y)) ||
                    IntersectWith(inputHelper.PointPosition))
                {
                    FireClick();
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
        Rectangle touchTap = new Rectangle((int)position.X - 1, (int)position.Y - 1, 2, 2);
        return Bounds.Intersects(touchTap);
    }

    /// <summary>
    /// Fires the button's click event.
    /// </summary>
    public void FireClick()
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
        SpriteBatch.Begin();

        SpriteBatch.Draw(_isPressed ? PressedTexture : RegularTexture, Bounds, Color.White);

        if (Font != null)
        {
            Vector2 textPosition = Font.MeasureString(Text);
            textPosition = new Vector2(Bounds.Width - textPosition.X,
                Bounds.Height - textPosition.Y);
            textPosition /= 2;
            textPosition.X += Bounds.X;
            textPosition.Y += Bounds.Y;
            SpriteBatch.DrawString(Font, Text, textPosition, Color.White);
        }

        SpriteBatch.End();

        base.Draw(gameTime);
    }
    #endregion

    protected override void Dispose(bool disposing)
    {
        Click = null;
        base.Dispose(disposing);
    }
}