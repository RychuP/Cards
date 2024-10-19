using Solitaire.Managers;
using Solitaire.Misc;
using Solitaire.UI.BaseScreens;
using Solitaire.UI.Buttons;

namespace Solitaire.UI.Screens;

internal class WinScreen : MenuScreen
{
    public Button RestartButton => GetButton(Strings.Restart);
    public Button ExitButton => GetButton(Strings.Exit);

    public WinScreen(GameManager gm) : base(gm, Art.WinTitle)
    {
        AddButton(Strings.Restart);
        AddButton(Strings.Exit);
    }
}