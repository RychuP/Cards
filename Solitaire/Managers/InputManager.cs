using Microsoft.Xna.Framework.Input;
using Solitaire.Misc;

namespace Solitaire.Managers;

/// <summary>
/// Keeps track of the input states in the previous frame.
/// </summary>
class InputManager : GameComponent
{
    public event EventHandler<ClickEventArgs> Click;

    /// <summary>
    /// Max distance from the initial mouse down position to class the event as a click.
    /// </summary>
    const int MouseClickPositionMaxOffset = 10;
    public static MouseState PrevMouseState { get; private set; }
    public static KeyboardState PrevKeyboardState { get; private set; }
    Point _mouseDownInitialPosition;
    public bool MouseIsDown { get; private set; }

    public InputManager(Game game) : base(game)
    {
        Game.Components.Add(this);
        UpdateOrder = int.MaxValue;
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
}