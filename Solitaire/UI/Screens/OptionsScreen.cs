using Solitaire.Managers;
using Solitaire.Misc;
using Solitaire.UI.AnimatedGameComponents;
using Solitaire.UI.BaseScreens;
using Solitaire.UI.Buttons;

namespace Solitaire.UI.Screens;

internal class OptionsScreen : MenuScreen
{
    public Button DifficultyButton => GetButton(GameManager.Difficulty.ToString());
    public Button ExitButton => GetButton(Strings.Exit);
    public OptionsScreen(GameManager gm) : base(gm, Art.OptionsTitle)
    {
        AddButton(new DifficultyButton(gm));
        AddButton(Strings.Exit);
    }
}