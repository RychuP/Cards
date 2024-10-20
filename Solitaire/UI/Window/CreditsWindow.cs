using Microsoft.Xna.Framework.Graphics;
using Solitaire.Managers;
using Solitaire.UI.BaseScreens;

namespace Solitaire.UI.Window;

internal class CreditsWindow : Window
{
    public CreditsWindow(Vector2 position, Rectangle bounds, MenuScreen menuScreen)
        : base(position, bounds, menuScreen)
    { }

    public override void Initialize()
    {
        AddText("Game written in C# and Monogame.");
        AddText("Author: Ryszard Pyka");
        AddText(" ");
        AddText("Several assets downloaded from:");
        AddText("www.textstudio.com");
        AddText("www.pngwing.com");
        AddText(" ");
        AddText("Source code available on github.");
        base.Initialize();
    }

    public override void Draw(GameTime gameTime)
    {
        var sb = Game.Services.GetService<SpriteBatch>();
        sb.Begin(SpriteSortMode.Immediate);

        foreach (var textLine in TextLines)
            sb.DrawString(GameManager.Font, textLine.Text, textLine.Position, Color.WhiteSmoke * Opacity);

        sb.End();
    }
}