using Microsoft.Xna.Framework.Input;

namespace Solitaire.Managers;

/// <summary>
/// Keeps track of the input states in the previous frame.
/// </summary>
class InputManager : GameComponent
{
    public static MouseState PrevMouseState { get; private set; }
    public static KeyboardState PrevKeyboardState { get; private set; }

    public InputManager(Game game) : base(game)
    {
        Game.Components.Add(this);
        UpdateOrder = int.MaxValue;
    }

    public override void Update(GameTime gameTime)
    {
        PrevMouseState = Mouse.GetState();
        PrevKeyboardState = Keyboard.GetState();
    }

    public static bool IsNewKeyPress(Keys key)
    {
        bool keyIsUpThisFrame = Keyboard.GetState().IsKeyUp(key);
        bool keyWasDownLastFrame = PrevKeyboardState.IsKeyDown(key);
        return keyIsUpThisFrame && keyWasDownLastFrame;
    }
}