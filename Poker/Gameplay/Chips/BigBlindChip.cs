namespace Poker.Gameplay.Chips;

class BigBlindChip : BlindChip
{
    public BigBlindChip(Game game, Vector2 position) : base(game, position, Art.BigBlindChip)
    {

    }

    public override Vector2 GetTablePosition()
    {
        int communityChipCount = Count + ValueChip.Count;
        int chipWidthWithPadding = Padding + Size.X;
        int offsetX = chipWidthWithPadding * (communityChipCount - 1);
        return base.GetTablePosition() + new Vector2(offsetX, 0);
    }
}