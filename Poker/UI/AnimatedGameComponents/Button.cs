using System;
using Framework.Assets;
using Framework.UI;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using InputButtonState = Microsoft.Xna.Framework.Input.ButtonState;

namespace Poker.UI.AnimatedGameComponents;

class Button : AnimatedGameComponent
{
    public event EventHandler Click;

    // text positions
    Vector2 _textPosition;
    Vector2 _regularFontTextPos;
    Vector2 _boldFontTextPos;
    Vector2 _boldFontWithOffsetTextPos;

    public PokerButtonState State { get; private set; }
    SpriteFont _currentFont;

    // constructor for buttons with variable position
    public Button(string text, Game game) : this(text, 0, game)
    { }

    // constructor for static buttons
    public Button(string text, int x, Game game) : base(game)
    {
        Text = text;
        ChangePosition(x);
        DrawOrder = int.MaxValue;
    }

    void SetTextPosition()
    {
        var dest = Destination ?? Rectangle.Empty;
        _regularFontTextPos = GetTextPosition(dest, Fonts.Moire.Regular.MeasureString(Text));
        _boldFontTextPos = GetTextPosition(dest, Fonts.Moire.Bold.MeasureString(Text));
        _boldFontWithOffsetTextPos = _boldFontTextPos + new Vector2(0, 2);
    }

    static Vector2 GetTextPosition(Rectangle dest, Vector2 textSize)
    {
        float x = dest.X + (Constants.ButtonWidth - textSize.X) / 2;
        float y = dest.Y + (Constants.ButtonSpriteHeight - textSize.Y) / 2;
        return new Vector2(x, y);
    }

    public void ChangePosition(int x)
    {
        Position = new Vector2(x, Constants.ButtonPositionY);
        Destination = new(x, Constants.ButtonPositionY, Constants.ButtonWidth, Constants.ButtonSpriteHeight);
        SetTextPosition();
    }

    protected override void LoadContent()
    {
        Texture = Art.ButtonSpriteSheet;
        _currentFont = Fonts.Moire.Regular;
    }

    public override void Update(GameTime gameTime)
    {
        HandleMouse();

        CurrentSegment = State switch
        {
            PokerButtonState.Hover => Constants.ButtonSpriteHoverSource,
            PokerButtonState.Pressed => Constants.ButtonSpritePressedSource,
            _ => Constants.ButtonSpriteRegularSource
        };

        _currentFont = State switch
        {
            PokerButtonState.Normal => Fonts.Moire.Regular,
            _ => Fonts.Moire.Bold
        };

        _textPosition = State switch
        {
            PokerButtonState.Hover => _boldFontTextPos,
            PokerButtonState.Pressed => _boldFontWithOffsetTextPos,
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
                InputButtonState.Pressed => PokerButtonState.Pressed,
                _ => PokerButtonState.Hover
            };

            // check for clicks
            if (mouseState.LeftButton == InputButtonState.Released &&
                InputManager.PrevMouseState.LeftButton == InputButtonState.Pressed)
            {
                OnClick();
            }
        }
        else
            State = PokerButtonState.Normal;
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
}