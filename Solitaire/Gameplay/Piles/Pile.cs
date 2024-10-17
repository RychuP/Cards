using Framework.Engine;
using Solitaire.Managers;
using Solitaire.Misc;
using Solitaire.UI.AnimatedGameComponents;

namespace Solitaire.Gameplay.Piles;

internal class Pile : Hand
{
    public AnimatedPile AnimatedPile { get; }

    /// <summary>
    /// Place that the pile occupies on the table.
    /// </summary>
    public Place Place { get; }

    public GameManager GameManager { get; }
    public Game Game => GameManager.Game;

    public Pile(GameManager gm, Place place, PileOrientation orientation, bool outlineVisible)
    {
        Place = place;
        GameManager = gm;
        AnimatedPile = new AnimatedPile(this, orientation, outlineVisible);
    }
}