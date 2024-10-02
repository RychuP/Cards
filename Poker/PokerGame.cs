global using Microsoft.Xna.Framework;
global using Poker.Misc;
using Framework.Engine;
using Poker.GameElements;
using Poker.UIElements;
using System;

namespace Poker;

class PokerGame : Game
{
    readonly GraphicsDeviceManager _graphicsDeviceManager;
    public GameManager GameManager { get; private set; }
    public ScreenManager ScreenManager { get; private set; }
    public Random Random { get; private set; }

    public PokerGame()
    {
        _graphicsDeviceManager = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        ScreenManager = new ScreenManager(this);
        Components.Add(new InputHelper(this));
        Components.Add(ScreenManager);
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        _graphicsDeviceManager.PreferredBackBufferWidth = Constants.GameWidth;
        _graphicsDeviceManager.PreferredBackBufferHeight = Constants.GameHeight;
        _graphicsDeviceManager.ApplyChanges();

        Art.Initialize(this);
        CardGame.Initialize(this);

        Random = new Random();
        GameManager = new GameManager(this);

        base.Initialize();
    }

    protected override void BeginRun()
    {
        ScreenManager.BeginRun();
        base.BeginRun();
    }

    static void Main()
    {
        using var game = new PokerGame();
        game.Run();
    }
}