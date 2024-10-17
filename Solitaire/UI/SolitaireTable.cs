using Framework.UI;
using Solitaire.Gameplay.Piles;
using Solitaire.Managers;
using Solitaire.Misc;
using Solitaire.UI.AnimatedGameComponents;
using Solitaire.UI.BaseScreens;

namespace Solitaire.UI;

internal class SolitaireTable : GameTable
{
    readonly static Vector2 PilePosition;
    readonly static Vector2 PileSpacing = new(AnimatedPile.OutlineWidth + AnimatedPile.Spacing, 0);

    GameManager GameManager { get; set; }

    static SolitaireTable()
    {
        int width = (int)PileSpacing.X * 7 - AnimatedPile.Spacing;
        int x = (SolitaireGame.Width - width) / 2;
        PilePosition = new Vector2(x, GameScreen.Margin);
    }

    public SolitaireTable(Game game) : 
        base(SolitaireGame.Bounds, Vector2.Zero, 1, GetPilePosition, game) 
    {

    }

    static Vector2 GetPilePosition(int pileIndex)
    {
        var place = (Place)pileIndex;

        // stock
        if (pileIndex == 0)
            return PilePosition;

        // waste
        if (pileIndex == 1)
            return PilePosition + PileSpacing;

        // foundations
        if (pileIndex >= 2 && pileIndex <= 5)
        {
            var horizontalOffset = PileSpacing * (pileIndex + 1);
            return PilePosition + horizontalOffset;
        }

        // tableaus
        else
        {
            int tableauIndex = pileIndex - 6;
            var verticalOffset = new Vector2(0, AnimatedPile.OutlineHeight + AnimatedPile.Spacing);
            var horizontalOffset = PileSpacing * tableauIndex;
            return PilePosition + verticalOffset + horizontalOffset;
                ;
        }
    }
}