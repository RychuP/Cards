using Microsoft.Xna.Framework.Input;
using Solitaire.Misc;

namespace Solitaire.Managers;

/// <summary>
/// Keeps track of the input states in the previous frame.
/// </summary>
class InputManager : GameComponent
{
    public event EventHandler<ClickEventArgs> Click;
    public event EventHandler EscapePressed;

    /// <summary>
    /// Max distance from the initial mouse down position to class the event as a click.
    /// </summary>
    const int MouseClickPositionMaxOffset = 10;
    public static MouseState PrevMouseState { get; private set; }
    public static KeyboardState PrevKeyboardState { get; private set; }
    Point _mouseDownInitialPosition;
    public bool MouseIsDown { get; private set; }
    public GameManager GameManager { get; }

    public InputManager(GameManager gm) : base(gm.Game)
    {
        GameManager = gm;
        Game.Components.Add(this);

        // make it update last
        UpdateOrder = int.MaxValue;
    }

    public override void Initialize()
    {
        // register event handlers
        GameManager.ScreenManager.ScreenChanged += ScreenManager_OnScreenChanged;
    }

    public override void Update(GameTime gameTime)
    {
        var mouseState = Mouse.GetState();

        // check left mouse button
        switch (mouseState.LeftButton)
        {
            case ButtonState.Pressed:
                if (!MouseIsDown)
                {
                    MouseIsDown = true;
                    _mouseDownInitialPosition = mouseState.Position;
                }
                break;

            case ButtonState.Released:
                if (MouseIsDown)
                {
                    MouseIsDown = false;
                    var dist = Vector2.Distance(_mouseDownInitialPosition.ToVector2(),
                        mouseState.Position.ToVector2());
                    if (dist <= MouseClickPositionMaxOffset)
                        OnClick(mouseState.Position);
                }
                break;
        }

        // check escape button
        if (IsNewKeyPress(Keys.Escape))
            OnEscapeButtonPressed();

        PrevMouseState = Mouse.GetState();
        PrevKeyboardState = Keyboard.GetState();
    }

    public static bool IsNewKeyPress(Keys key)
    {
        bool keyIsUpThisFrame = Keyboard.GetState().IsKeyUp(key);
        bool keyWasDownLastFrame = PrevKeyboardState.IsKeyDown(key);
        return keyIsUpThisFrame && keyWasDownLastFrame;
    }

    void OnClick(Point position)
    {
        var args = new ClickEventArgs(position);
        Click?.Invoke(this, args);
    }

    void OnEscapeButtonPressed()
    {
        EscapePressed?.Invoke(this, EventArgs.Empty);
    }

    void ScreenManager_OnScreenChanged(object o, ScreenChangedEventArgs e)
    {
        // prevent invoking clicks on mouse button release when changing screens
        MouseIsDown = false;
    }
}