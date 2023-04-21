using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Poker.UI;
using System.Collections.Generic;

namespace Poker.Screens;

/// <summary>
/// The screen manager is a component which manages one or more GameScreen instances. 
/// It maintains a stack of screens, calls their Update and Draw methods at the appropriate times, 
/// and automatically routes input to the topmost active screen.
/// </summary>
internal class ScreenManager : DrawableGameComponent
{
    readonly List<GameScreen> _screens = new();

    public ScreenManager(Game game) : base(game)
    {
        AddScreen(new BackgroundScreen());
    }

    /// <summary>
    /// Default <see cref="SpriteBatch"/> shared by all the <see cref="GameScreen"/> objects.
    /// </summary>
    public SpriteBatch SpriteBatch { get; private set; }

    /// <summary>
    /// Default <see cref="SpriteFont"/> shared by all the <see cref="GameScreen"/> objects.
    /// </summary>
    public SpriteFont Font { get; private set; }

    public override void Initialize()
    {
        Game.Components.Add(new Button("Raise", Game));
        foreach (GameScreen screen in _screens)
            screen.Initialize();

        base.Initialize();
    }

    protected override void LoadContent()
    {
        SpriteBatch = new SpriteBatch(GraphicsDevice);
        Font = Game.Content.Load<SpriteFont>("Fonts/MenuFont");

        // let each of the screens load their content
        foreach (GameScreen screen in _screens)
            screen.LoadContent();
    }

    protected override void UnloadContent()
    {
        foreach (GameScreen screen in _screens)
            screen.UnloadContent();
    }

    public override void Update(GameTime gameTime)
    {
        foreach (GameScreen screen in _screens)
            if (screen.Enabled)
                screen.Update(gameTime);
    }

    public override void Draw(GameTime gameTime)
    {
        foreach (GameScreen screen in _screens)
            if (screen.Visible)
                screen.Draw(gameTime);
    }

    public void AddScreen(GameScreen screen)
    {
        _screens.Add(screen);
        screen.ScreenManager = this;
    }
}