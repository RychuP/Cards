namespace Solitaire.Misc;

internal static class Extensions
{
    public static void Disable(this GameComponent component) =>
        component.Enabled = false;
    public static void Enable(this GameComponent component) =>
        component.Enabled = true;
    public static void Show(this DrawableGameComponent component) =>
        component.Visible = true;
    public static void Hide(this DrawableGameComponent component) =>
        component.Visible = false;
}