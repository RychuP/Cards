using Microsoft.Xna.Framework;
using Poker.Screens;
using Poker.UI;

namespace Poker;

class PokerGame : Game
{
    readonly GraphicsDeviceManager _graphicsDeviceManager;

    public PokerGame()
    {
        Content.RootDirectory = "Content";
        _graphicsDeviceManager = new GraphicsDeviceManager(this);
        Components.Add(new InputHelper(this));
        Components.Add(new ScreenManager(this));
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        _graphicsDeviceManager.PreferredBackBufferWidth = Constants.GameWidth;
        _graphicsDeviceManager.PreferredBackBufferHeight = Constants.GameHeight;
        _graphicsDeviceManager.ApplyChanges();

        Art.Initialize(this);
        base.Initialize();
    }

    protected override void BeginRun()
    {
        Components.Find<ScreenManager>().ShowScreen<MainMenuScreen>();
        base.BeginRun();
    }

    static void Main()
    {
        using var game = new PokerGame();
        game.Run();
    }
}