using Solitaire.Managers;
using Solitaire.Misc;
using Solitaire.UI.BaseScreens;
using Solitaire.UI.Buttons;
using Solitaire.UI.Window;

namespace Solitaire.UI.Screens;

internal class CreditsScreen : MenuScreen
{
    readonly CreditsWindow _window;
    public Button ExitButton => GetButton(Strings.Exit);

    public CreditsScreen(GameManager gm) : base(gm, Art.CreditsTitle, Direction.Up)
    {
        _window = new CreditsWindow(Title.DarkAreaPosition, Title.DarkArea.Bounds, this);
        AddButton(Strings.Exit);

        // register event handlers
        //Title.DestinationReached += Title_OnDestinationReached;
    }

    public override void Initialize()
    {
        base.Initialize();
        Game.Components.Add(_window);
    }

    //void Title_OnDestinationReached(object o, DirectionEventArgs e)
    //{
    //    if (e.Direction == Direction.Up)
    //        _window.Show();
    //}

    //protected override void OnVisibleChanged(object o, EventArgs e)
    //{
    //    if (Visible == false && _window is not null)
    //        _window.Hide();
    //    base.OnVisibleChanged(o, e);
    //}
}