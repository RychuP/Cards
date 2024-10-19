using Microsoft.Xna.Framework.Graphics;
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

        // create screens
        var startScreen = new StartScreen(gm);
        var optionsScreen = new OptionsScreen(gm);
        var pauseScreen = new PauseScreen(gm);
        var winScreen = new WinScreen(gm);
        AddScreen(startScreen);
        AddScreen(optionsScreen);
        AddScreen(pauseScreen);
        AddScreen(winScreen);
        AddScreen(new GameplayScreen(gm));

        // show start screen
        ShowScreen<StartScreen>();

        // register event handlers
        GameManager.InputManager.EscapePressed += InputManager_OnEscapeButtonPressed;
        GameManager.WinRule.RuleMatch += (o, e) => ShowScreen<WinScreen>();
        startScreen.StartButton.Click += (o, e) => ShowScreen<GameplayScreen>();
        startScreen.OptionsButton.Click += (o, e) => ShowScreen<OptionsScreen>();
        optionsScreen.ExitButton.Click += (o, e) => ShowScreen<StartScreen>();
        pauseScreen.ContinueButton.Click += (o, e) => ShowScreen<GameplayScreen>();
        pauseScreen.ExitButton.Click += (o, e) => ShowScreen<StartScreen>();
        winScreen.RestartButton.Click += (o, e) => ShowScreen<GameplayScreen>();
        winScreen.ExitButton.Click += (o, e) => ShowScreen<StartScreen>();
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

    public static Texture2D CreateTexture(GraphicsDevice device, int width, int height, Color color)
    {
        //initialize a texture
        Texture2D texture = new(device, width, height);

        //the array holds the color for each pixel in the texture
        Color[] data = new Color[width * height];
        Array.Fill(data, color);

        //set the color
        texture.SetData(data);

        return texture;
    }

    public void OnScreenChanged(GameScreen prevScreen, GameScreen newScreen)
    {
        prevScreen?.Hide();
        newScreen.Show();
        ScreenChanged?.Invoke(this, new ScreenChangedEventArgs(prevScreen, newScreen));
    }

    void InputManager_OnEscapeButtonPressed(object o, EventArgs e)
    {
        switch (Screen)
        {
            case StartScreen:
                Game.Exit();
                break;
            case GameplayScreen:
                ShowScreen<PauseScreen>();
                break;
            default:
                ShowScreen<StartScreen>();
                break;
        }
    }
}