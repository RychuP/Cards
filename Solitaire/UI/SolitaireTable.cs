using Framework.UI;
using Solitaire.Gameplay;

namespace Solitaire.UI;

internal class SolitaireTable : GameTable
{
    GameManager GameManager { get; set; }

    public SolitaireTable(Game game) : 
        base(SolitaireGame.Bounds, Vector2.Zero, 1, (x) => Vector2.Zero, game) 
    {
        
    }
}