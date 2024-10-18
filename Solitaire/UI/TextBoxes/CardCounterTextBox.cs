using Solitaire.Gameplay.Piles;
using Solitaire.Managers;

namespace Solitaire.UI.TextBoxes;

internal class CardCounterTextBox : TextBox
{
    static readonly int Padding = 1;
    readonly Pile _pile;

    public CardCounterTextBox(Pile pile, GameManager gm) : base(gm)
    {
        _pile = pile;
        Visible = false;

        // calculate destination
        var textSize = gm.Font.MeasureString("00");
        int width = pile.Bounds.Width;
        int height = (int)textSize.Y + Padding * 2;
        int x = (int)pile.Position.X;
        int y = (int)pile.Position.Y - height - 2;
        Bounds = new Rectangle(x, y, width, height);
    }

    public override void Update(GameTime gameTime)
    {
        Text = _pile.Count.ToString();
    }
}