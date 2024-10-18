using Solitaire.Managers;
using Solitaire.Misc;
using Solitaire.UI;
using Solitaire.UI.AnimatedGameComponents;
using Solitaire.UI.BaseScreens;
using Solitaire.UI.Screens;

namespace Solitaire.Gameplay.Piles;

internal class Tableau : Pile
{
    public Tableau(GameManager gm, Place place) : base(gm, place, true)
    {
        Point size = new(Bounds.Width, SolitaireGame.Height - (int)Position.Y - GameScreen.HorizontalMargin);
        Bounds = new Rectangle(Bounds.Location, size);

        AnimatedPile = new AnimatedTableau(this);

        
    }

    
}