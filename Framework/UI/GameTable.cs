using Microsoft.Xna.Framework;

namespace Framework.UI;

/// <summary>
/// The UI representation of the table where the game is played.
/// </summary>
public class GameTable : DrawableGameComponent
{
    public Vector2 DealerPosition { get; private set; }
    public Func<int, Vector2> PlaceOrder { get; private set; }
    public Rectangle TableBounds { get; private set; }
    public int Places { get; private set; }

    /// <summary>
    /// Returns the player position on the table according to the player index.
    /// </summary>
    /// <param name="index">Player's index.</param>
    /// <returns>The position of the player corresponding to the supplied index.</returns>
    /// <remarks>The location's are relative to the entire game area, even
    /// if the table only occupies part of it.</remarks>
    public Vector2 this[int index] =>
        new Vector2(TableBounds.Left, TableBounds.Top) + PlaceOrder(index);

    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>
    /// <param name="tableBounds">The table bounds.</param>
    /// <param name="dealerPosition">The dealer's position.</param>
    /// <param name="places">Amount of places on the table</param>
    /// <param name="placeOrder">A method to convert player indices to their
    /// respective location on the table.</param>
    /// <param name="game">The associated game object.</param>
    public GameTable(Rectangle tableBounds, Vector2 dealerPosition, int places,
        Func<int, Vector2> placeOrder, Game game) : base(game)
    {
        TableBounds = tableBounds;
        DealerPosition = dealerPosition + new Vector2(tableBounds.Left, tableBounds.Top);
        Places = places;
        PlaceOrder = placeOrder;
    }

    public virtual void Show()
    {
        Visible = true;
        Enabled = true;
    }

    public virtual void Hide()
    {
        Visible = false;
        Enabled = false;
    }
}