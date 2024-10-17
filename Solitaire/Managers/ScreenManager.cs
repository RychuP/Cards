using Solitaire.Misc;
using Solitaire.UI.BaseScreens;
using Solitaire.UI.Screens;
using System.Collections.Generic;

namespace Solitaire.Managers;

internal class ScreenManager
{
    public event EventHandler<ScreenChangedEventArgs> ScreenChanged;

    readonly List<GameScreen> _screens = new();
    public GameManager GameManager { get; }
    public Game Game => GameManager.Game;

    // backing field
    GameScreen _activeScreen;
    /// <summary>
    /// Currently displayed screen.
    /// </summary>
    public GameScreen Screen
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

    public ScreenManager(GameManager gm)
    {
        GameManager = gm;
        GameManager.EscapePressed += GameManager_OnEscapeButtonPressed;

        var gameplayScreen = new StartScreen(gm);
        gameplayScreen.GetButton(Strings.Start).Click += StartButton_OnClick;
        AddScreen(gameplayScreen);

        AddScreen(new GameplayScreen(gm));
        ShowScreen<StartScreen>();
    }

    void AddScreen(GameScreen screen)
    {
        _screens.Add(screen);
        Game.Components.Add(screen);
    }

    public void ShowScreen<T>() where T : GameScreen
    {
        var screen = GetScreen<T>();
        if (screen is not null)
            Screen = screen;
    }

    public T GetScreen<T>() where T : GameScreen =>
        _screens.Find(t => t is T) as T;

    public void OnScreenChanged(GameScreen prevScreen, GameScreen newScreen)
    {
        prevScreen?.Hide();
        newScreen.Show();
        ScreenChanged?.Invoke(this, new ScreenChangedEventArgs(prevScreen, newScreen));
    }

    void StartButton_OnClick(object o, EventArgs e) =>
        ShowScreen<GameplayScreen>();

    void GameManager_OnEscapeButtonPressed(object o, EventArgs e)
    {
        if (Screen is GameplayScreen)
            ShowScreen<StartScreen>();
    }
}