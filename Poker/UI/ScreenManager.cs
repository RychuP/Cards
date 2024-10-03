using System;
using System.Collections.Generic;
using Poker.Gameplay;
using Poker.UI.ScreenElements;
using Poker.UI.Screens;

namespace Poker.UI;

/// <summary>
/// The screen manager is a component which manages one or more GameScreen instances. 
/// It maintains a stack of screens, calls their Update and Draw methods at the appropriate times, 
/// and automatically routes input to the topmost active screen.
/// </summary>
class ScreenManager : GameComponent
{
    GameScreen _activeScreen;
    readonly List<GameScreen> _screens = new();

    public GameScreen ActiveScreen
    {
        get => _activeScreen;
        set
        {
            if (_activeScreen == value) return;
            var prevActScreen = _activeScreen;
            _activeScreen = value;
            OnScreenChanged(prevActScreen, value);
        }
    }

    public ScreenManager(Game game) : base(game)
    {
        AddScreen(new BackgroundScreen(this));
        AddScreen(new MainMenuScreen(this));
        AddScreen(new ThemeScreen(this));
        AddScreen(new GameplayScreen(this));
        AddScreen(new PauseScreen(this));
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        ((PokerGame)Game).GameManager.Update();
    }

    void AddScreen(GameScreen screen)
    {
        _screens.Add(screen);
        Game.Components.Add(screen);
    }

    public void ShowScreen<T>() where T : GameScreen
    {
        var screen = _screens.Find(t => t is T);
        if (screen is not null)
            ActiveScreen = screen;
    }

    /// <summary>
    /// Final call before the first Update.
    /// </summary>
    public void BeginRun()
    {
        ShowScreen<MainMenuScreen>();
    }

    public void OnScreenChanged(GameScreen prevScreen, GameScreen newScreen)
    {
        prevScreen?.Hide();
        newScreen.Show();

        ScreenChanged?.Invoke(this, new ScreenChangedEventArgs(prevScreen, newScreen));
    }

    public event EventHandler<ScreenChangedEventArgs> ScreenChanged;
}