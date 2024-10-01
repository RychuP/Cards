using Microsoft.Xna.Framework;
using System.Linq;

namespace Poker;

static class Extensions
{
    /// <summary>
    /// Finds a <see cref="GameComponent"/> of given type in the collection.
    /// </summary>
    public static T Find<T>(this GameComponentCollection components) =>
        components.Where(t => t is T).Select(t => (T)t).FirstOrDefault();

    /// <summary>
    /// Moves the <see cref="Rectangle"/> by a given offset given in <see cref="Vector2"/>.
    /// </summary>
    /// <param name="offset"><see cref="Vector2"/> that is added to the <see cref="Rectangle"/> position.</param>
    public static Rectangle Move(this Rectangle rect, Vector2 offset) =>
        new(rect.X + (int)offset.X, rect.Y + (int)offset.Y, rect.Width, rect.Height);

    /// <summary>
    /// Moves the <see cref="Rectangle"/> by a given offset given in <paramref name="x"/> and <paramref name="y"/>.
    /// </summary>
    /// <param name="x">X offset.</param>
    /// <param name="y">Y offset.</param>
    public static Rectangle Move(this Rectangle rect, int x, int y) =>
        new(rect.X + x, rect.Y + y, rect.Width, rect.Height);
}