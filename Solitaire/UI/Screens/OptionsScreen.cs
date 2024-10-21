using Solitaire.Managers;
using Solitaire.Misc;
using Solitaire.UI.BaseScreens;
using Solitaire.UI.Buttons;
using Solitaire.UI.Window;

namespace Solitaire.UI.Screens;

internal class OptionsScreen : MenuScreen
{
    readonly OptionsWindow _window;
    public Button ThemeButton => GetButton(GameManager.Theme);
    public Button DifficultyButton => GetButton(GameManager.Difficulty.ToString());
    public Button ExitButton => GetButton(Strings.Exit);

    public OptionsScreen(GameManager gm) : base(gm, Art.OptionsTitle, Direction.Up)
    {
        _window = new OptionsWindow(Title.DarkAreaPosition, Title.DarkArea.Bounds, this);
        AddButton(new ThemeButton(gm));
        AddButton(new DifficultyButton(gm));
        AddButton(Strings.Exit);
    }

    public override void Initialize()
    {
        base.Initialize();
        Game.Components.Add(_window);
    }
}