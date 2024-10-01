global using Microsoft.Xna.Framework;
global using Poker.Misc;

using Poker.GameElements;
using Poker.UIElements;

namespace Poker;

class PokerGame : Game
{
    readonly GraphicsDeviceManager _graphicsDeviceManager;
    public GameManager GameManager { get; private set; }
    public ScreenManager ScreenManager { get; private set; }

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

        GameManager = new GameManager(this);
        Art.Initialize(this);
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