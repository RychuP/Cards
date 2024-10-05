global using Microsoft.Xna.Framework;
global using Poker.Misc;
using Framework.Engine;
using Poker.Gameplay;
using Poker.UI;
using System;

namespace Poker;

class PokerGame : Game
{
    readonly GraphicsDeviceManager _graphicsDeviceManager;

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
        Services.AddService(new ScreenManager(this));

        Components.Add(new InputHelper(this));
        Services.AddService(new GameManager(this));

        base.Initialize();
    }

    protected override void Update(GameTime gameTime)
    {
        Services.GetService<GameManager>().Update();
        base.Update(gameTime);
    }

    protected override void BeginRun()
    {
        Services.GetService<ScreenManager>().BeginRun();
        base.BeginRun();
    }

    static void Main()
    {
        using var game = new PokerGame();
        game.Run();
    }
}