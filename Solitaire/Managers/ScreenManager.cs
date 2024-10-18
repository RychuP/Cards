using Microsoft.Xna.Framework.Graphics;
using Solitaire.Misc;
using Solitaire.UI.BaseScreens;
using Solitaire.UI.Screens;
using System.Collections.Generic;
using System.Linq;

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

        var startScreen = new StartScreen(gm);
        startScreen.GetButton(Strings.Start).Click += StartScreen_StartButton_OnClick;
        startScreen.GetButton(Strings.Options).Click += StartScreen_OptionsButton_OnClick;
        AddScreen(startScreen);

        var optionsScreen = new OptionsScreen(gm);
        optionsScreen.GetButton(Strings.Exit).Click += OptionsScreen_ExitButton_OnClick;
        AddScreen(optionsScreen);

        var pauseScreen = new PauseScreen(gm);
        pauseScreen.GetButton(Strings.Continue).Click += PauseScreen_ContinueButton_OnClick;
        pauseScreen.GetButton(Strings.Exit).Click += PauseScreen_ExitButton_OnClick;
        AddScreen(pauseScreen);

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

    public static Texture2D CreateTexture(GraphicsDevice device, int width, int height, Color color)
    {
        //initialize a texture
        Texture2D texture = new Texture2D(device, width, height);

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

    void GameManager_OnEscapeButtonPressed(object o, EventArgs e)
    {
        if (Screen is GameplayScreen)
            ShowScreen<PauseScreen>();
        else if (Screen is PauseScreen || Screen is OptionsScreen)
            ShowScreen<StartScreen>();
    }

    void StartScreen_StartButton_OnClick(object o, EventArgs e) =>
        ShowScreen<GameplayScreen>();

    void StartScreen_OptionsButton_OnClick(object o, EventArgs e) =>
        ShowScreen<OptionsScreen>();

    void PauseScreen_ExitButton_OnClick(object o, EventArgs e) =>
        ShowScreen<StartScreen>();

    void PauseScreen_ContinueButton_OnClick(object o, EventArgs e) =>
        ShowScreen<GameplayScreen>();

    void OptionsScreen_ExitButton_OnClick(object o, EventArgs e) =>
        ShowScreen<StartScreen>();
}