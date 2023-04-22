using Microsoft.Xna.Framework;
using Poker.Screens;
using Poker.UI;

namespace Poker;

public class PokerGame : Game
{
    public const int Width = 1280;
    public const int Height = 720;
    public static readonly Rectangle Area = new(0, 0, Width, Height);

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
        _graphics.PreferredBackBufferWidth = Width; 
        _graphics.PreferredBackBufferHeight = Height;
        _graphics.ApplyChanges();
        base.Initialize();
    }

    static void Main()
    {
        using var game = new PokerGame();
        game.Run();
    }
}