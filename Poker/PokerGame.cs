global using Microsoft.Xna.Framework;
global using Poker.Misc;

using Framework.Engine;
using Poker.Gameplay;
using Poker.UI;
using System;
using System.Collections.Generic;

namespace Poker;

class PokerGame : Game
{
    readonly GraphicsDeviceManager _graphicsDeviceManager;
    readonly List<IGlobalManager> _globalManagers = new();

    public PokerGame()
    {
        _graphicsDeviceManager = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        _graphicsDeviceManager.PreferredBackBufferWidth = Constants.GameWidth;
        _graphicsDeviceManager.PreferredBackBufferHeight = Constants.GameHeight;
        _graphicsDeviceManager.ApplyChanges();

        Art.Initialize(this);
        CardGame.Initialize(this);

        Services.AddService(new Random());

        // screen manager needs to be created first to get the lowest draw order
        var screenManager = new ScreenManager(this);
        Services.AddService(screenManager);

        var inputManager = new InputManager(this);
        Services.AddService(inputManager);

        var gameManager = new GameManager(this, screenManager);
        Services.AddService(gameManager);

        _globalManagers.Add(gameManager);
        _globalManagers.Add(inputManager);

        base.Initialize();
    }

    protected override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        _globalManagers.ForEach(manager => manager.Update());
    }

    protected override void BeginRun()
    {
        Services.GetService<GameManager>().AssignEventHandlers();
        Services.GetService<ScreenManager>().BeginRun();
        base.BeginRun();
    }

    static void Main()
    {
        using var game = new PokerGame();
        game.Run();
    }
}

interface IGlobalManager
{
    Game Game { get; }
    void Update();
}