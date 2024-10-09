using System;
using System.Collections.Generic;
using Poker.UI.BaseScreens;
using Poker.UI.Screens;

namespace Poker.UI;

/// <summary>
/// The screen manager is a component which manages one or more GameScreen instances.
/// </summary>
class ScreenManager
{
    readonly List<GameScreen> _screens = new();
    GameScreen _activeScreen;
    public Game Game { get; }

    public GameScreen ActiveScreen
    {
        get => _activeScreen;
        private set
        {
            if (_activeScreen == value) return;
            var prevActScreen = _activeScreen;
            _activeScreen = value;
            OnScreenChanged(prevActScreen, value);
        }
    }

    public ScreenManager(Game game)
    {
        Game = game;
        AddScreen(new BackgroundScreen(this));
        AddScreen(new StartScreen(this));
        AddScreen(new ThemeScreen(this));
        AddScreen(new GameplayScreen(this));
        AddScreen(new PauseScreen(this));
        AddScreen(new TestScreen(this));
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
        ShowScreen<StartScreen>();
    }

    public void OnScreenChanged(GameScreen prevScreen, GameScreen newScreen)
    {
        prevScreen?.Hide();
        newScreen.Show();

        ScreenChanged?.Invoke(this, new ScreenChangedEventArgs(prevScreen, newScreen));
    }

    public event EventHandler<ScreenChangedEventArgs> ScreenChanged;
}