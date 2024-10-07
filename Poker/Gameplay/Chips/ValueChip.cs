using Framework.Assets;
using System;

namespace Poker.Gameplay.Chips;

class ValueChip : Chip
{
    /// <summary>
    /// Available values for the <see cref="ValueChip"/>.
    /// </summary>
    public static readonly int[] Values = ChipAssets.Values;

    /// <summary>
    /// Number of available <see cref="ValueChip"/>.
    /// </summary>
    public static readonly int Count = Values.Length;

    /// <summary>
    /// Y coordinate of the first value chip placed below the community cards.
    /// </summary>
    public static readonly int DefaultPositionY = 480;

    /// <summary>
    /// Vertical offset between chips placed below the community cards.
    /// </summary>
    public static readonly int VerticalOffset = 12;

    /// <summary>
    /// Value of the chip.
    /// </summary>
    public int Value { get; }

    /// <summary>
    /// Index of the chip in the <see cref="Values"/> array.
    /// </summary>
    public int Index { get; }

    public ValueChip(Game game, Vector2 position, int value)
        : base(game, position, value switch
        {
            5 => ChipAssets.ValueChips[5],
            25 => ChipAssets.ValueChips[25],
            100 => ChipAssets.ValueChips[100],
            500 => ChipAssets.ValueChips[500],
            _ => throw new NotImplementedException()
        })
    {
        Value = value;

        // save index
        int index = -1;
        for (int i = 0; i < Count; i++)
            if (Values[i] == value)
                index = i;
        if (index != -1)
            Index = index;
        else
            throw new ArgumentException($"Unrecognised chip value {value}.");
    }

    /// <summary>
    /// Calculates the position for the chips below the community cards.
    /// </summary>
    /// <returns></returns>
    public override Vector2 GetTablePosition()
    {
        int widthCombined = Size.X * Count;
        int paddingCombined = (Count - 1) * Padding;
        int initialX = (Constants.GameWidth - widthCombined - paddingCombined) / 2;
        int offsetX = (Padding + Size.X) * Index;
        int offsetY = Index switch
        {
            1 => VerticalOffset,
            2 => VerticalOffset,
            _ => 0
        };

        return new Vector2(initialX + offsetX, DefaultPositionY + offsetY);
    }
}