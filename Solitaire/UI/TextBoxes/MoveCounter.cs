using Solitaire.Gameplay.Piles;

namespace Solitaire.UI.TextBoxes;

internal class MoveCounter : TextBox
{
    int _count = 1;
    int Count
    {
        get => _count;
        set
        {
            if (_count == value) return;
            if (value < 0)
                throw new ArgumentOutOfRangeException("value");
            if (value > _count + 1)
                throw new ArgumentException("Count should only go up in the increments of 1.");
            _count = value;
            OnCountChanged(value);
        }
    }

    public MoveCounter(Waste waste) : base(waste.GameManager)
    {
        Visible = false;

        // calculate text box size
        var textSize = GameManager.Font.MeasureString("00");
        int width = Pile.OutlineWidth * 2 + Pile.OutlineSpacing.X;
        int height = (int)textSize.Y + Padding * 2;

        // calculate position
        int x = (int)GameManager.Tableaus[1].Position.X;
        int y = (int)waste.Position.Y - height - 2;
        Bounds = new Rectangle(x, y, width, height);

        // reset counter and its associated text
        Count = 0;

        // register event handlers
        GameManager.GameInit += (o, e) => Count = 0;
        GameManager.Stock.MoveMade += Pile_OnMoveMade;
        GameManager.Waste.MoveMade += Pile_OnMoveMade;
        foreach (var foundation in GameManager.Foundations)
            foundation.MoveMade += Pile_OnMoveMade;
        foreach (var tableau in GameManager.Tableaus)
            tableau.MoveMade += Pile_OnMoveMade;
    }

    void OnCountChanged(int newValue)
    {
        string s = newValue == 1 ? string.Empty : "s";
        Text = $"{newValue} move{s}";
    }

    void Pile_OnMoveMade(object o, EventArgs e)
    {
        Count++;
    }
}