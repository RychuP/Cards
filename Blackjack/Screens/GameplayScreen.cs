#region File Description
//-----------------------------------------------------------------------------
// GameplayScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using GameStateManagement;
using Microsoft.Xna.Framework;
using CardsFramework;

namespace Blackjack;

class GameplayScreen : GameScreen
{
    #region Fields and Properties
    BlackjackCardGame _blackJackGame;

    InputHelper _inputHelper;

    readonly string _theme;
    readonly List<DrawableGameComponent> _pauseEnabledComponents = new();
    readonly List<DrawableGameComponent> _pauseVisibleComponents = new();
    Rectangle _safeArea;

    readonly static Vector2[] _playerCardOffset = new Vector2[] 
    { 
        new Vector2(100f * BlackjackGame.WidthScale, 190f * BlackjackGame.HeightScale),
        new Vector2(336f * BlackjackGame.WidthScale, 210f * BlackjackGame.HeightScale),
        new Vector2(570f * BlackjackGame.WidthScale, 190f * BlackjackGame.HeightScale) 
    };
    #endregion

    #region Initializations
    /// <summary>
    /// Initializes a new instance of the screen.
    /// </summary>
    public GameplayScreen(string theme)
    {
        _theme = theme;
        TransitionOnTime = TimeSpan.FromSeconds(0.0);
        TransitionOffTime = TimeSpan.FromSeconds(0.5);
    }
    #endregion

    #region Loading
    /// <summary>
    /// Load content and initializes the actual game.
    /// </summary>
    public override void LoadContent()
    {
        _safeArea = ScreenManager.SafeArea;

        // Initialize virtual cursor
        _inputHelper = new(ScreenManager.Game)
        {
            DrawOrder = 1000
        };
        ScreenManager.Game.Components.Add(_inputHelper);

        _inputHelper.Visible = false;
        _inputHelper.Enabled = false;

        _blackJackGame = new BlackjackCardGame(ScreenManager.GraphicsDevice.Viewport.Bounds,
            new Vector2(_safeArea.Left + _safeArea.Width / 2 - 50, _safeArea.Top + 20),
            GetPlayerCardPosition, ScreenManager, _theme);


        InitializeGame();

        base.LoadContent();
    }

    /// <summary>
    /// Unload content loaded by the screen.
    /// </summary>
    public override void UnloadContent()
    {
        ScreenManager.Game.Components.Remove(_inputHelper);

        base.UnloadContent();
    }
    #endregion

    #region Update and Render
    /// <summary>
    /// Handle user input.
    /// </summary>
    /// <param name="input">User input information.</param>
    public override void HandleInput(InputState input)
    {
        if (input.IsPauseGame(null))
        {
            PauseCurrentGame();
        }

        base.HandleInput(input);
    }

    /// <summary>
    /// Perform the screen's update logic.
    /// </summary>
    /// <param name="gameTime">The time that has passed since the last call to 
    /// this method.</param>
    /// <param name="otherScreenHasFocus">Whether or not another screen has
    /// the focus.</param>
    /// <param name="coveredByOtherScreen">Whether or not another screen covers
    /// this one.</param>
    public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
    {
        if (_blackJackGame != null && !coveredByOtherScreen)
            _blackJackGame.Update();
        base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
    }

    /// <summary>
    /// Draw the screen
    /// </summary>
    /// <param name="gameTime"></param>
    public override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);
        _blackJackGame?.Draw(gameTime);
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Initializes the game component.
    /// </summary>
    private void InitializeGame()
    {
        _blackJackGame.Initialize();

        // Add human player
        _blackJackGame.AddPlayer(new BlackjackPlayer("Abe", _blackJackGame));

        // Add AI players
        BlackjackAIPlayer player = new("Benny", _blackJackGame);
        _blackJackGame.AddPlayer(player);
        player.Hit += Player_OnHit;
        player.Stand += Player_OnStand;

        player = new BlackjackAIPlayer("Chuck", _blackJackGame);
        _blackJackGame.AddPlayer(player);
        player.Hit += Player_OnHit;
        player.Stand += Player_OnStand;

        // Load UI assets
        string[] assets = { "blackjack", "bust", "lose", "push", "win", "pass", "shuffle_" + _theme };

        for (int chipIndex = 0; chipIndex < assets.Length; chipIndex++)
        {
            _blackJackGame.LoadUITexture("UI", assets[chipIndex]);
        }

        _blackJackGame.StartRound();
    }

    /// <summary>
    /// Gets the player hand positions according to the player index.
    /// </summary>
    /// <param name="player">The player's index.</param>
    /// <returns>The position for the player's hand on the game table.</returns>
    private Vector2 GetPlayerCardPosition(int player)
    {
        switch (player)
        {
            case 0:
            case 1:
            case 2:
                return new Vector2(ScreenManager.SafeArea.Left,
                    ScreenManager.SafeArea.Top + 200 * (BlackjackGame.HeightScale - 1)) +
                    _playerCardOffset[player];
            default:
                throw new ArgumentException(
                    "Player index should be between 0 and 2", "player");
        }
    }

    /// <summary>
    /// Pause the game.
    /// </summary>
    private void PauseCurrentGame()
    {
        // Move to the pause screen
        ScreenManager.AddScreen(new BackgroundScreen(), null);
        ScreenManager.AddScreen(new PauseScreen(), null);

        // Hide and disable all components which are related to the gameplay screen
        _pauseEnabledComponents.Clear();
        _pauseVisibleComponents.Clear();
        foreach (IGameComponent component in ScreenManager.Game.Components)
        {
            if (component is BetGameComponent ||
                component is AnimatedGameComponent ||
                component is GameTable ||
                component is InputHelper)
            {
                DrawableGameComponent pauseComponent = (DrawableGameComponent)component;
                if (pauseComponent.Enabled)
                {
                    _pauseEnabledComponents.Add(pauseComponent);
                    pauseComponent.Enabled = false;
                }
                if (pauseComponent.Visible)
                {
                    _pauseVisibleComponents.Add(pauseComponent);
                    pauseComponent.Visible = false;
                }
            }
        }
    }

    /// <summary>
    /// Returns from pause.
    /// </summary>
    public void ReturnFromPause()
    {
        // Reveal and enable all previously hidden components
        foreach (DrawableGameComponent component in _pauseEnabledComponents)
            component.Enabled = true;

        foreach (DrawableGameComponent component in _pauseVisibleComponents)
            component.Visible = true;
    }
    #endregion

    #region Event Handler
    /// <summary>
    /// Responds to the event sent when AI player's choose to "Stand".
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The 
    /// <see cref="System.EventArgs"/> instance containing the event data.</param>
    void Player_OnStand(object sender, EventArgs e)
    {
        _blackJackGame.Stand();
    }

    /// <summary>
    /// Responds to the event sent when AI player's choose to "Split".
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The 
    /// <see cref="System.EventArgs"/> instance containing the event data.</param>
    void Player_OnSplit(object sender, EventArgs e)
    {
        _blackJackGame.Split();
    }

    /// <summary>
    /// Responds to the event sent when AI player's choose to "Hit".
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    void Player_OnHit(object sender, EventArgs e)
    {
        _blackJackGame.Hit();
    }

    /// <summary>
    /// Responds to the event sent when AI player's choose to "Double".
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    void Player_OnDouble(object sender, EventArgs e)
    {
        _blackJackGame.Double();
    }
    #endregion
}