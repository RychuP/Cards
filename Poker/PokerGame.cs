using Microsoft.Xna.Framework;
using Poker.Screens;
using Poker.UI;

namespace Poker;

public class PokerGame : Game
{
    readonly GraphicsDeviceManager _graphics;
    readonly ScreenManager _screenManager;

    public PokerGame()
    {
        _graphics = new GraphicsDeviceManager(this);
        _screenManager = new(this);
        Content.RootDirectory = "Content";
        Components.Add(new InputHelper(this));
        Components.Add(_screenManager);
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        _graphics.PreferredBackBufferWidth = 1280; 
        _graphics.PreferredBackBufferHeight = 720;
        _graphics.ApplyChanges();
        base.Initialize();
    }

    static void Main()
    {
        using var game = new PokerGame();
        game.Run();
    }
}