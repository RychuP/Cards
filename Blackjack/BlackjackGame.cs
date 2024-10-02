using Microsoft.Xna.Framework;
using Framework.Engine;
using Blackjack.Misc;
using Blackjack.UI.Screens;
using Blackjack.UI;

namespace Blackjack;

public class BlackjackGame : Game
{
    readonly GraphicsDeviceManager _graphics;
    ScreenManager _screenManager;

    public static float HeightScale { get; private set; } = 1.0f;
    public static float WidthScale { get; private set; } = 1.0f;

    /// <summary>
    /// Initializes a new instance of the game.
    /// </summary>
    public BlackjackGame()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        _screenManager = new ScreenManager(this);
        _screenManager.AddScreen(new BackgroundScreen(), null);
        _screenManager.AddScreen(new MainMenuScreen(), null);
        Components.Add(_screenManager);
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

        Art.Initialize(this);
        CardGame.Initialize(this);
    }

    static void Main()
    {
        using var game = new BlackjackGame();
        game.Run();
    }
}