using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Poker.Screens;

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
            OnActiveScreenChanged(prevActScreen, value);
        }
    }

    public ScreenManager(Game game) : base(game)
    {
        AddScreen(new BackgroundScreen(this));
        AddScreen(new MainMenuScreen(this));
        AddScreen(new GameplayScreen(this));
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

    static void OnActiveScreenChanged(GameScreen prevActScreen, GameScreen newActScreen)
    {
        prevActScreen?.Hide();
        newActScreen.Show();
    }
}