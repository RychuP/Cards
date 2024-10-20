using static System.Net.Mime.MediaTypeNames;

namespace Solitaire.UI.Window;

internal class TextLine
{
    public Vector2 Position { get; private set; }
    public Vector2 Size { get; private set; }
    readonly Window _window;

    string _text;
    public string Text
    {
        get => _text;
        set
        {
            if (_text == value) return;
            var prevText = _text;
            _text = value;
            OnTextChanged(prevText, value);
        }
    }

    public TextLine(string text, int y, Window window) : this(text, window)
    {
        Position = new Vector2(Position.X, y);
    }

    public TextLine(string text, Window window)
    {
        _window = window;
        Text = text;

        // calculate position based on the number of previous text lines
        var lineCount = window.TextLines.Count;
        var y = window.Position.Y + Window.Margin + Size.Y * lineCount + Window.LineSpacing * lineCount;
        Position = new Vector2(Position.X, y);

    }

    void OnTextChanged(string prevText, string newText)
    {
        var size = _window.GameManager.Font.MeasureString(newText);
        if (Size != size)
        {
            Size = size;
            var x = _window.Position.X + ((_window.Size.X - Size.X) / 2);
            Position = new Vector2(x, Position.Y);
        }
    }
}