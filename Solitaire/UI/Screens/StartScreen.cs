using Solitaire.Managers;
using Solitaire.Misc;
using Solitaire.UI.BaseScreens;

namespace Solitaire.UI.Screens;

internal class StartScreen : MenuScreen
{
    public StartScreen(GameManager gm) : base(gm, Art.SolitaireTitle)
    {
        AddButton(Strings.Start);
        AddButton(Strings.Options);
        AddButton(Strings.Exit);
    }
}