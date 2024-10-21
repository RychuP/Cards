using Framework.Misc;
using Solitaire.Managers;

namespace Solitaire.UI.Buttons;

internal class ThemeButton : Button
{
    public ThemeButton(GameManager gm) : base(gm.Theme.ToString(), gm)
    {
        gm.ThemeChanged += GameManager_OnThemeChanged;
    }

    void GameManager_OnThemeChanged(object o, ThemeChangedEventArgs e)
    {
        Text = e.NewTheme;
    }
}