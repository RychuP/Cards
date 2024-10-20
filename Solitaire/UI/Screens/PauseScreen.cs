using Solitaire.Managers;
using Solitaire.Misc;
using Solitaire.UI.BaseScreens;
using Solitaire.UI.Buttons;

namespace Solitaire.UI.Screens;

internal class PauseScreen : MenuScreen
{
    public Button ContinueButton => GetButton(Strings.Continue);
    public Button ExitButton => GetButton(Strings.Exit);

    public PauseScreen(GameManager gm) : base(gm, Art.PauseTitle, Direction.Down)
    {
        AddButton(Strings.Continue);
        AddButton(Strings.Exit);
    }
}