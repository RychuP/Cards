namespace Poker.Gameplay.Chips;

class BlindChip : Chip
{
    public BlindChip(Game game, Vector2 position, BlindType type) 
        : base(game, position, type == BlindType.Small ? Art.SmallBlindChip : Art.BigBlindChip)
    {

    }
}