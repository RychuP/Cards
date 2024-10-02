using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Blackjack.UI.ScreenElements;
using Blackjack.Misc;

namespace Blackjack.UI;

/// <summary>
/// The screen manager is a component which manages one or more GameScreen
/// instances. It maintains a stack of screens, calls their Update and Draw
/// methods at the appropriate times, and automatically routes input to the
/// topmost active screen.
/// </summary>
public class ScreenManager : DrawableGameComponent
{
    bool _isInitialized;
    readonly List<GameScreen> _screens = new();
    readonly List<GameScreen> _screensToUpdate = new();
    public InputState Input { get; init; } = new();

    /// <summary>
    /// If true, the manager prints out a list of all the screens
    /// each time it is updated. This can be useful for making sure
    /// everything is being added and removed at the right times.
    /// </summary>
    public bool TraceEnabled { get; set; } = false;

    /// <summary>
    /// Returns the portion of the screen where drawing is safely allowed.
    /// </summary>
    public Rectangle SafeArea =>
        Game.GraphicsDevice.Viewport.TitleSafeArea;

    /// <summary>
    /// Constructs a new screen manager component.
    /// </summary>
    public ScreenManager(Game game) : base(game) { }

    /// <summary>
    /// Initializes the screen manager component.
    /// </summary>
    public override void Initialize()
    {
        base.Initialize();
        _isInitialized = true;
    }

    /// <summary>
    /// Load your graphics content.
    /// </summary>
    protected override void LoadContent()
    {
        // Tell each of the screens to load their content.
        foreach (GameScreen screen in _screens)
            screen.LoadContent();
    }

    /// <summary>
    /// Unload your graphics content.
    /// </summary>
    protected override void UnloadContent()
    {
        // Tell each of the screens to unload their content.
        foreach (GameScreen screen in _screens)
            screen.UnloadContent();
    }

    /// <summary>
    /// Allows each screen to run logic.
    /// </summary>
    public override void Update(GameTime gameTime)
    {
        // Read the keyboard and gamepad.
        Input.Update();

        // Make a copy of the master screen list, to avoid confusion if
        // the process of updating one screen adds or removes others.
        _screensToUpdate.Clear();

        foreach (GameScreen screen in _screens)
            _screensToUpdate.Add(screen);

        bool otherScreenHasFocus = !Game.IsActive;
        bool coveredByOtherScreen = false;

        // Loop as long as there are screens waiting to be updated.
        while (_screensToUpdate.Count > 0)
        {
            // Pop the topmost screen off the waiting list.
            int i = _screensToUpdate.Count - 1;
            GameScreen screen = _screensToUpdate[i];
            _screensToUpdate.RemoveAt(i);

            // Update the screen.
            screen.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            if (screen.ScreenState == ScreenState.TransitionOn ||
                screen.ScreenState == ScreenState.Active)
            {
                // If this is the first active screen we came across,
                // give it a chance to handle input.
                if (!otherScreenHasFocus)
                {
                    screen.HandleInput(Input);
                    otherScreenHasFocus = true;
                }

                // If this is an active non-popup, inform any subsequent
                // screens that they are covered by it.
                if (!screen.IsPopup)
                    coveredByOtherScreen = true;
            }
        }

        // Print debug trace?
        if (TraceEnabled)
            TraceScreens();
    }

    /// <summary>
    /// Prints a list of all the screens, for debugging.
    /// </summary>
    void TraceScreens()
    {
        List<string> screenNames = new();

        foreach (GameScreen screen in _screens)
            screenNames.Add(screen.GetType().Name);

        Debug.WriteLine(string.Join(", ", screenNames.ToArray()));
    }

    /// <summary>
    /// Tells each screen to draw itself.
    /// </summary>
    public override void Draw(GameTime gameTime)
    {
        foreach (GameScreen screen in _screens)
            if (screen.ScreenState != ScreenState.Hidden)
                screen.Draw(gameTime);
    }

    /// <summary>
    /// Adds a new screen to the screen manager.
    /// </summary>
    public void AddScreen(GameScreen screen, PlayerIndex? controllingPlayer)
    {
        screen.ControllingPlayer = controllingPlayer;
        screen.ScreenManager = this;
        screen.IsExiting = false;

        // If we have a graphics device, tell the screen to load content.
        if (_isInitialized)
            screen.LoadContent();

        _screens.Add(screen);
    }

    /// <summary>
    /// Removes a screen from the screen manager. You should normally
    /// use GameScreen.ExitScreen instead of calling this directly, so
    /// the screen can gradually transition off rather than just being
    /// instantly removed.
    /// </summary>
    public void RemoveScreen(GameScreen screen)
    {
        // If we have a graphics device, tell the screen to unload content.
        if (_isInitialized)
            screen.UnloadContent();

        _screens.Remove(screen);
        _screensToUpdate.Remove(screen);
    }

    /// <summary>
    /// Expose an array holding all the screens. We return a copy rather
    /// than the real master list, because screens should only ever be added
    /// or removed using the AddScreen and RemoveScreen methods.
    /// </summary>
    public GameScreen[] GetScreens() =>
        _screens.ToArray();

    /// <summary>
    /// Helper draws a translucent black fullscreen sprite, used for fading
    /// screens in and out, and for darkening the background behind popups.
    /// </summary>
    public void FadeBackBufferToBlack(float alpha)
    {
        Viewport viewport = GraphicsDevice.Viewport;
        var sb = Game.Services.GetService(typeof(SpriteBatch)) as SpriteBatch;
        sb.Begin();
        sb.Draw(Art.Blank, new Rectangle(0, 0, viewport.Width, viewport.Height), Color.Black * alpha);
        sb.End();
    }
}