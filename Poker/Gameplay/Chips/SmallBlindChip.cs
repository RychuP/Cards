namespace Poker.Gameplay.Chips;

class SmallBlindChip : BlindChip
{
    public SmallBlindChip(Game game) : base(game, HiddenPosition, Art.SmallBlindChip)
    { }
}