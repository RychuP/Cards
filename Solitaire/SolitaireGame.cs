global using System;
global using Microsoft.Xna.Framework;
using Framework.Engine;
using Solitaire.UI.BaseScreens;
using Solitaire.Managers;
using Solitaire.UI;
using Solitaire.UI.AnimatedGameComponents;
using Microsoft.Xna.Framework.Input;

namespace Solitaire;

public class SolitaireGame : Game
{
    public static readonly int Width = (GameScreen.Margin * 2) +
        (AnimatedPile.OutlineWidth + AnimatedPile.Spacing) * 7 - AnimatedPile.Spacing;
    public static readonly int Height = (GameScreen.Margin * 2) +
        (AnimatedPile.OutlineHeight * 2) + AnimatedPile.Spacing + 12 * AnimatedPile.VerticalCardSpacing;
    public static readonly Rectangle Bounds = new(0, 0, Width, Height);

    readonly GraphicsDeviceManager _graphics;
    GameManager _gameManager;

    public SolitaireGame()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        _graphics.PreferredBackBufferWidth = Width;
        _graphics.PreferredBackBufferHeight = Height;
        _graphics.ApplyChanges();

        Art.Initialize(this);
        CardGame.Initialize(this);
        _gameManager = new GameManager(this);

        base.Initialize();
    }

    protected override void Update(GameTime gameTime)
    {
        _gameManager.Update(gameTime);
        base.Update(gameTime);
    }

    static void Main()
    {
        using var game = new SolitaireGame();
        game.Run();
    }
}