using Microsoft.Xna.Framework.Input;
using System.Threading;

namespace Poker.UI;

/// <summary>
/// Keeps track of the input states in the previous frame.
/// </summary>
class InputManager : IGlobalManager
{
    public Game Game { get; }
    public static MouseState PrevMouseState { get; private set; }
    public static KeyboardState PrevKeyboardState {  get; private set; }

    public InputManager(Game game)
    {
        Game = game;
        Update();
    }

    public void Update()
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