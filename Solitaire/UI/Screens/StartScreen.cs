using Solitaire.Managers;
using Solitaire.Misc;
using Solitaire.UI.BaseScreens;
using Solitaire.UI.Buttons;

namespace Solitaire.UI.Screens;

internal class StartScreen : MenuScreen
{
    public Button StartButton => GetButton(Strings.Start);
    public Button OptionsButton => GetButton(Strings.Options);
    public Button ExitButton => GetButton(Strings.Exit);

    public StartScreen(GameManager gm) : base(gm, Art.SolitaireTitle)
    {
        AddButton(Strings.Start);
        AddButton(Strings.Options);
        AddButton(Strings.Exit);
    }
}