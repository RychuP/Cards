using Solitaire.Managers;
using Solitaire.Misc;
using Solitaire.UI.BaseScreens;

namespace Solitaire.UI.Screens;

internal class PauseScreen : MenuScreen
{
    public PauseScreen(GameManager gm) : base(gm, Art.PauseTitle)
    {
        AddButton(Strings.Continue);
        AddButton(Strings.Exit);
    }
}