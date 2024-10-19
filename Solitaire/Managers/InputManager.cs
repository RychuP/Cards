using Microsoft.Xna.Framework.Input;
using Solitaire.Misc;

namespace Solitaire.Managers;

/// <summary>
/// Keeps track of the input states in the previous frame.
/// </summary>
class InputManager : GameComponent
{
    public event EventHandler<PointEventArgs> Click;
    public event EventHandler<PointEventArgs> LeftMouseButtonDown;
    public event EventHandler<PointEventArgs> LeftMouseButtonReleased;
    public event EventHandler EscapePressed;

    /// <summary>
    /// Max distance from the initial mouse down position to class the event as a click.
    /// </summary>
    const int MaxPositionOffset = 4;
    public MouseState PrevMouseState { get; private set; }
    public KeyboardState PrevKeyboardState { get; private set; }

    /// <summary>
    /// Position saved when the left mouse button was started to be held down.
    /// </summary>
    public Point MouseDownInitialPosition { get; private set; }

    /// <summary>
    /// Current mouse position.
    /// </summary>
    public Point MousePosition { get; private set; }

    /// <summary>
    /// Left mouse button is down.
    /// </summary>
    public bool MouseIsDown { get; private set; }

    /// <summary>
    /// True if the mouse is moving with the left button down.
    /// </summary>
    public bool IsDragging => MouseIsDown && 
        DistanceToInitialMouseDownPosition > MaxPositionOffset;

    /// <summary>
    /// Distance to initial mouse down position. Don't call unless IsDragging checked first.
    /// </summary>
    public float DistanceToInitialMouseDownPosition =>
        Vector2.Distance(MouseDownInitialPosition.ToVector2(), MousePosition.ToVector2());

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
        MousePosition = mouseState.Position;

        // check left mouse button
        switch (mouseState.LeftButton)
        {
            case ButtonState.Pressed:
                if (!MouseIsDown)
                {
                    MouseIsDown = true;
                    MouseDownInitialPosition = MousePosition;
                    OnLeftMouseButtonDown(MousePosition);
                }
                break;

            case ButtonState.Released:
                if (MouseIsDown)
                {
                    if (!IsDragging)
                        OnClick(MousePosition);
                    OnLeftMouseButtonReleased(MousePosition);
                    MouseIsDown = false;
                }
                break;
        }

        // check escape button
        if (IsNewKeyPress(Keys.Escape))
            OnEscapeButtonPressed();

        PrevMouseState = Mouse.GetState();
        PrevKeyboardState = Keyboard.GetState();
    }

    public bool IsNewKeyPress(Keys key)
    {
        bool keyIsUpThisFrame = Keyboard.GetState().IsKeyUp(key);
        bool keyWasDownLastFrame = PrevKeyboardState.IsKeyDown(key);
        return keyIsUpThisFrame && keyWasDownLastFrame;
    }

    void OnEscapeButtonPressed()
    {
        EscapePressed?.Invoke(this, EventArgs.Empty);
    }

    void OnClick(Point position)
    {
        var args = new PointEventArgs(position);
        Click?.Invoke(this, args);
    }

    void OnLeftMouseButtonDown(Point position)
    {
        var args = new PointEventArgs(position);
        LeftMouseButtonDown?.Invoke(this, args);
    }

    void OnLeftMouseButtonReleased(Point position)
    {
        var args = new PointEventArgs(position);
        LeftMouseButtonReleased?.Invoke(this, args);
    }

    void ScreenManager_OnScreenChanged(object o, ScreenChangedEventArgs e)
    {
        // prevent invoking clicks on mouse button release when changing screens
        MouseIsDown = false;
    }
}