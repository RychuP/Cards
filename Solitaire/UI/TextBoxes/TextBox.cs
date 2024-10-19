using Microsoft.Xna.Framework.Graphics;
using Solitaire.Managers;
using Solitaire.Misc;

namespace Solitaire.UI.TextBoxes;

internal class TextBox : DrawableGameComponent
{
    public string Text { get; set; }
    public TextAlignment TextAlignment { get; set; }
    public Rectangle Bounds { get; set; }
    public GameManager GameManager { get; }
    public Color Color { get; set; }

    public TextBox(GameManager gm) : this(string.Empty, Rectangle.Empty, gm)
    { }

    public TextBox(Rectangle destination, GameManager gm) : this(string.Empty, destination, gm)
    { }

    public TextBox(string text, Rectangle destination, GameManager gm) : base(gm.Game)
    {
        Text = text;
        TextAlignment = TextAlignment.Centered;
        Bounds = destination;
        Color = Color.Black;
        GameManager = gm;
        Game.Components.Add(this);
    }

    public override void Draw(GameTime gameTime)
    {
        if (Bounds == Rectangle.Empty || Text == string.Empty) return;

        var sb = Game.Services.GetService<SpriteBatch>();
        sb.Begin();

        var texture = ScreenManager.CreateTexture(Game.GraphicsDevice, Bounds.Width,
            Bounds.Height, Color.ForestGreen);
        sb.Draw(texture, Bounds, Color.White);

        var textSize = GameManager.Font.MeasureString(Text);
        float offsetY = (Bounds.Height - textSize.Y) / 2;
        float offsetX = TextAlignment switch
        {
            TextAlignment.RightAligned => Bounds.Width - textSize.X,
            TextAlignment.Centered => (Bounds.Width - textSize.X) / 2,
            _ => 0
        };
        var textPosition = Bounds.Location.ToVector2() + new Vector2(offsetX, offsetY);
        sb.DrawString(GameManager.Font, Text, textPosition, Color);

        sb.End();
    }
}