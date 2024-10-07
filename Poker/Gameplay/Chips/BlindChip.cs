using Microsoft.Xna.Framework.Graphics;
using System;

namespace Poker.Gameplay.Chips;

abstract class BlindChip : Chip
{
    /// <summary>
    /// Number of available blind chips.
    /// </summary>
    public static readonly int Count = 2;

    /// <summary>
    /// Vertical distance between the regular, betting chip position 
    /// and the blind chip position in the player area.
    /// </summary>
    public static readonly int OffsetY = 37;

    /// <summary>
    /// Vertical offset between chips placed below the community cards.
    /// </summary>
    public static readonly int VerticalOffset = 30;

    public BlindChip(Game game, Vector2 position, Texture2D texture) 
        : base(game, position, texture)
    {

    }

    /// <summary>
    /// Calculates the table position for <see cref="SmallBlindChip"/>.
    /// </summary>
    /// <returns></returns>
    public override Vector2 GetTablePosition()
    {
        int communityChipCount = Count + ValueChip.Count;
        int widthCombined = Size.X * communityChipCount;
        int paddingCombined = (communityChipCount - 1) * Padding;
        int initialX = (Constants.GameWidth - widthCombined - paddingCombined) / 2;
        return new Vector2(initialX, ValueChip.DefaultPositionY - VerticalOffset);
    }
}