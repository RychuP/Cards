using Framework.Assets;
using Framework.UI;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Solitaire.Managers;
using Solitaire.Misc;

namespace Solitaire.UI.Buttons;

internal class Button : AnimatedGameComponent
{
    public event EventHandler Click;

    // static constants
    public static readonly int Height = 64;
    public static readonly int Width = 200;
    public static readonly int Spacing = 20;
    public static readonly int PositionY = SolitaireGame.Height - Height - Spacing * 2;
    public static readonly Rectangle SpriteRegularSource = new(0, 0, Width, Height);
    public static readonly Rectangle SpriteHoverSource = new(0, Height, Width, Height);
    public static readonly Rectangle SpritePressedSource = new(0, Height * 2, Width, Height);

    // text positions
    Vector2 _textPosition;
    Vector2 _regularFontTextPos;
    Vector2 _boldFontTextPos;
    Vector2 _boldFontWithOffsetTextPos;

    // font used
    SpriteFont _currentFont;

    public GameButtonState State { get; private set; }
    GameManager GameManager { get; }

    // constructor for buttons with variable position
    public Button(string text, GameManager gm) : this(text, 0, gm)
    { }

    // constructor for static buttons
    public Button(string text, int x, GameManager gm) : base(gm.Game)
    {
        Text = text;
        GameManager = gm;
        SetPosX(x);
        DrawOrder = int.MaxValue;
        Hide();
    }

    void CalculateTextPositions()
    {
        var dest = Destination ?? Rectangle.Empty;
        _regularFontTextPos = GetTextPosition(dest, Fonts.Moire.Regular.MeasureString(Text));
        _boldFontTextPos = GetTextPosition(dest, Fonts.Moire.Bold.MeasureString(Text));
        _boldFontWithOffsetTextPos = _boldFontTextPos + new Vector2(0, 2);
    }

    static Vector2 GetTextPosition(Rectangle dest, Vector2 textSize)
    {
        float x = dest.X + (Width - textSize.X) / 2;
        float y = dest.Y + (Height - textSize.Y) / 2;
        return new Vector2(x, y);
    }

    public void SetPosX(int x)
    {
        Position = new Vector2(x, PositionY);
        Destination = new(x, PositionY, Width, Height);
        CalculateTextPositions();
    }

    protected override void LoadContent()
    {
        Texture = Art.Buttons;
        _currentFont = Fonts.Moire.Regular;
    }

    public override void Update(GameTime gameTime)
    {
        HandleMouse();

        CurrentSegment = State switch
        {
            GameButtonState.Hover => SpriteHoverSource,
            GameButtonState.Pressed => SpritePressedSource,
            _ => SpriteRegularSource
        };

        _currentFont = State switch
        {
            GameButtonState.Normal => Fonts.Moire.Regular,
            _ => Fonts.Moire.Bold
        };

        _textPosition = State switch
        {
            GameButtonState.Hover => _boldFontTextPos,
            GameButtonState.Pressed => _boldFontWithOffsetTextPos,
            _ => _regularFontTextPos
        };

        base.Update(gameTime);
    }

    public override void Draw(GameTime gameTime)
    {
        if (Destination is not Rectangle dest) return;
        var sb = Game.Services.GetService<SpriteBatch>();
        sb.Begin();
        sb.Draw(Texture, dest, CurrentSegment, Color.White);
        sb.DrawString(_currentFont, Text, _textPosition, Color.White);
        sb.End();
    }

    void HandleMouse()
    {
        if (Destination is not Rectangle dest) return;

        var mouseState = Mouse.GetState();
        if (dest.Contains(mouseState.Position))
        {
            // change the button state depending on the mouse left button state
            State = mouseState.LeftButton switch
            {
                ButtonState.Pressed => GameButtonState.Pressed,
                _ => GameButtonState.Hover
            };

            // check for clicks
            if (mouseState.LeftButton == ButtonState.Released &&
                GameManager.InputManager.PrevMouseState.LeftButton == ButtonState.Pressed)
            {
                OnClick();
            }
        }
        else
            State = GameButtonState.Normal;
    }

    /// <summary>
    /// Sets <see cref="Visible"/> and <see cref="Enabled"/> to true.
    /// </summary>
    public void Show()
    {
        Visible = true;
        Enabled = true;
    }

    /// <summary>
    /// Sets <see cref="Visible"/> and <see cref="Enabled"/> to false.
    /// </summary>
    public void Hide()
    {
        Visible = false;
        Enabled = false;
    }

    void OnClick()
    {
        Click?.Invoke(this, EventArgs.Empty);
    }

    protected override void OnTextChanged(string prevText, string newText)
    {
        CalculateTextPositions();
        base.OnTextChanged(prevText, newText);
    }
}