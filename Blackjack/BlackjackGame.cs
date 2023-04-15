#region File Description
//-----------------------------------------------------------------------------
// PauseScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

using Microsoft.Xna.Framework;
using GameStateManagement;

namespace Blackjack;

/// <summary>
/// This is the main game type.
/// </summary>
public class BlackjackGame : Game
{
    readonly GraphicsDeviceManager _graphics;
    readonly ScreenManager _screenManager;

    public static float HeightScale = 1.0f;
    public static float WidthScale = 1.0f;

    /// <summary>
    /// Initializes a new instance of the game.
    /// </summary>
    public BlackjackGame()
    {
        _graphics = new GraphicsDeviceManager(this);

        Content.RootDirectory = "Content";

        _screenManager = new ScreenManager(this);

        _screenManager.AddScreen(new BackgroundScreen(), null);
        _screenManager.AddScreen(new MainMenuScreen(), null);

        Components.Add(_screenManager);

        IsMouseVisible = true;

        // Initialize sound system
        AudioManager.Initialize(this);
    }

    protected override void Initialize()
    {
        base.Initialize();

        _graphics.PreferredBackBufferHeight = 480;
        _graphics.PreferredBackBufferWidth = 800; 
  
        _graphics.ApplyChanges();

        Rectangle bounds = _graphics.GraphicsDevice.Viewport.TitleSafeArea;
        HeightScale = bounds.Height / 480f;
        WidthScale = bounds.Width / 800f;
    }

    /// <summary>
    /// LoadContent will be called once per game and is the place to load
    /// all of your content.
    /// </summary>
    protected override void LoadContent()
    {
        AudioManager.LoadSounds();
        base.LoadContent();
    }
}
