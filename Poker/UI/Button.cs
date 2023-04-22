using System;
using CardsFramework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MouseButtonState = Microsoft.Xna.Framework.Input.ButtonState;

namespace Poker.UI;

internal class Button : AnimatedGameComponent
{
    // segment size
    const int SegmentWidth = 291;
    const int SegmentHeight = 64;

    // button size
    public const int Width = 175;
    public const int Height = SegmentHeight;

    // button state segments
    readonly Rectangle _normalSegment = new(0, 0, SegmentWidth, SegmentHeight);
    readonly Rectangle _hoverSegment = new(0, SegmentHeight, SegmentWidth, SegmentHeight);
    readonly Rectangle _pressedSegment = new(0, SegmentHeight * 2, SegmentWidth, SegmentHeight);

    // button state (mouse controlled)
    public ButtonState State { get; private set; }

    // fonts
    SpriteFont _fontBold;
    SpriteFont _fontRegular;
    SpriteFont _currentFont;

    // text location
    Vector2 _textPosition;

    public Button(string text, int x, int y, Game game) : this(text, game)
    {
        Destination = new(x, y, Width, Height);
    }

    public Button(string text, Game game) : base(game)
    {
        Text = text;
        Visible = false;
        Enabled = false;
    }

    protected override void LoadContent()
    {
        Texture = Game.Content.Load<Texture2D>("Images/UI/buttons");
        _fontBold = Game.Content.Load<SpriteFont>("Fonts/Bold");
        _currentFont = _fontRegular = Game.Content.Load<SpriteFont>("Fonts/Regular");
    }

    public override void Update(GameTime gameTime)
    {
        if (Destination is not Rectangle dest) return;

        HandleMouse();

        CurrentSegment = State switch
        {
            ButtonState.Hover => _hoverSegment,
            ButtonState.Pressed => _pressedSegment,
            _ => _normalSegment
        };

        _currentFont = State switch
        {
            ButtonState.Normal => _fontRegular,
            _ => _fontBold
        };

        var textSize = _currentFont.MeasureString(Text);
        float x = dest.X + (Width - textSize.X) / 2;
        float y = dest.Y + (Height - textSize.Y) / 2;

        _textPosition = State switch
        {
            ButtonState.Pressed => new(x, y + 2),
            _ => new(x, y)
        };
        
        base.Update(gameTime);
    }

    public override void Draw(GameTime gameTime)
    {
        if (Destination is not Rectangle dest) return;

        SpriteBatch.Begin();

        // draw button
        SpriteBatch.Draw(Texture, dest, CurrentSegment, Color.White);

        // draw text
        SpriteBatch.DrawString(_currentFont, Text, _textPosition, Color.White);

        SpriteBatch.End();
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
                MouseButtonState.Pressed => ButtonState.Pressed,
                _ => ButtonState.Hover
            };

            // check for clicks
            if (mouseState.LeftButton == MouseButtonState.Released &&
                InputHelper.PrevMouseState.LeftButton == MouseButtonState.Pressed)
            {
                OnClick();
            }
        }
        else
            State = ButtonState.Normal;
    }

    void OnClick()
    {
        Click?.Invoke(this, EventArgs.Empty);
    }

    public event EventHandler Click;
}