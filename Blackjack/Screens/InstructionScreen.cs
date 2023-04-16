#region File Description
//-----------------------------------------------------------------------------
// InstructionScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using GameStateManagement;

namespace Blackjack;

class InstructionScreen : GameplayScreen
{
    #region Fields
    Texture2D _background;
    SpriteFont _font;
    GameplayScreen _gameplayScreen;
    readonly string _theme;
    bool _isExit = false;
    bool _isExited = false;
    #endregion

    #region Initialization
    public InstructionScreen(string theme) : base("")
    {
        _theme = theme;
        TransitionOnTime = TimeSpan.FromSeconds(0.0);
        TransitionOffTime = TimeSpan.FromSeconds(0.5);
    }
    #endregion

    #region Loading
    /// <summary>
    /// Load the screen resources
    /// </summary>
    public override void LoadContent()
    {
        _background = Load<Texture2D>(@"Images\instructions");
        _font = Load<SpriteFont>(@"Fonts\MenuFont");

        // Create a new instance of the gameplay screen
        _gameplayScreen = new GameplayScreen(_theme);
    }
    #endregion

    #region Update and Render
    /// <summary>
    /// Exit the screen after a tap or click
    /// </summary>
    /// <param name="input"></param>
    private void HandleInput(MouseState mouseState, GamePadState padState)
    {
        if (!_isExit)
        {
            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                _isExit = true;
            }
            else if (ScreenManager.Input.IsNewButtonPress(Buttons.A, null, out _) ||
                ScreenManager.Input.IsNewButtonPress(Buttons.Start, null, out _))
            {
                _isExit = true;
            }
        }
    }

    /// <summary>
    /// Screen update logic
    /// </summary>
    /// <param name="gameTime"></param>
    /// <param name="otherScreenHasFocus"></param>
    /// <param name="coveredByOtherScreen"></param>
    public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
    {
        if (_isExit && !_isExited)
        {
            // Move on to the gameplay screen
            foreach (GameScreen screen in ScreenManager.GetScreens())
                screen.ExitScreen();

            _gameplayScreen.ScreenManager = ScreenManager;
            ScreenManager.AddScreen(_gameplayScreen, null);
            _isExited = true;
        }

        HandleInput(Mouse.GetState(), GamePad.GetState(PlayerIndex.One));

        base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
    }

    /// <summary>
    /// Render screen 
    /// </summary>
    /// <param name="gameTime"></param>
    public override void Draw(GameTime gameTime)
    {
        SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

        spriteBatch.Begin();

        // Draw Background
        spriteBatch.Draw(_background, ScreenManager.GraphicsDevice.Viewport.Bounds,
             Color.White * TransitionAlpha);

        if (_isExit)
        {
            Rectangle safeArea = ScreenManager.SafeArea;
            string text = "Loading...";
            Vector2 measure = _font.MeasureString(text);
            Vector2 textPosition = new(safeArea.Center.X - measure.X / 2,
                safeArea.Center.Y - measure.Y / 2);
            spriteBatch.DrawString(_font, text, textPosition, Color.Black);
        }

        spriteBatch.End();
        base.Draw(gameTime);
    }
    #endregion
}