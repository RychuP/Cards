using Framework.Assets;

namespace Poker.Gameplay.Chips;

class ValueChip : Chip
{
    readonly int Value;

    public ValueChip(Game game, Vector2 position, int value)
        : base(game, position, value switch
        {
            5 => ChipAssets.ValueChips[5],
            25 => ChipAssets.ValueChips[25],
            100 => ChipAssets.ValueChips[100],
            _ => ChipAssets.ValueChips[500],
        })
    {
        Value = value;
    }
}