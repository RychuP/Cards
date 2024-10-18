using Solitaire.Managers;
using Solitaire.Misc;
using Solitaire.UI.AnimatedGameComponents;
using Solitaire.UI.BaseScreens;

namespace Solitaire.UI.Screens;

internal class OptionsScreen : MenuScreen
{
    public OptionsScreen(GameManager gm) : base(gm, Art.OptionsTitle)
    {
        AddButton(new DifficultyButton(gm));
        AddButton(Strings.Exit);
    }
}