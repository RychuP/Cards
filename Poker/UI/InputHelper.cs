using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Poker.UI;

/// <summary>
/// Keeps track of the input states in the previous frame.
/// </summary>
internal class InputHelper : GameComponent
{
    public static MouseState PrevMouseState { get; private set; }

    public InputHelper(Game game) : base(game)
    {
        UpdateOrder = int.MaxValue;
        PrevMouseState = Mouse.GetState();
    }

    public override void Update(GameTime gameTime)
    {
        PrevMouseState = Mouse.GetState(); 
    }
}