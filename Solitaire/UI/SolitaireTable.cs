using Framework.UI;
using Solitaire.Gameplay.Piles;
using Solitaire.Managers;
using Solitaire.Misc;
using Solitaire.UI.BaseScreens;

namespace Solitaire.UI;

internal class SolitaireTable : GameTable
{
    readonly static Vector2 StockPosition;
    readonly static Vector2 PileOffset = new(Pile.OutlineWidth + Pile.OutlineSpacing.X, 0);

    GameManager GameManager { get; set; }

    static SolitaireTable()
    {
        // calculate the width of all piles
        int width = (int)PileOffset.X * 7 - Pile.OutlineSpacing.X;
        int x = (SolitaireGame.Width - width) / 2;

        // save the stock position
        StockPosition = new Vector2(x, Pile.OutlineSpacing.Y + GameScreen.TopMargin);
    }

    public SolitaireTable(Game game) : 
        base(SolitaireGame.Bounds, Vector2.Zero, 1, GetPilePosition, game) 
    { }

    static Vector2 GetPilePosition(int pileIndex)
    {
        var place = (PilePlace)pileIndex;

        // stock
        if (pileIndex == 0)
            return StockPosition;

        // waste
        if (pileIndex == 1)
            return StockPosition + PileOffset - new Vector2(Pile.OutlineSpacing.X / 2, 0);

        // foundations
        if (pileIndex >= 2 && pileIndex <= 5)
        {
            var horizontalOffset = PileOffset * (pileIndex + 1);
            return StockPosition + horizontalOffset;
        }

        // tableaus
        else
        {
            int tableauIndex = pileIndex - 6;
            var verticalOffset = new Vector2(0, Pile.OutlineHeight + Pile.OutlineSpacing.Y);
            var horizontalOffset = PileOffset * tableauIndex;
            return StockPosition + verticalOffset + horizontalOffset;
        }
    }
}