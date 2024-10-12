using Framework.Assets;
using Framework.UI;
using Microsoft.Xna.Framework.Graphics;
using Poker.Gameplay;
using Poker.Gameplay.Players;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Poker.UI.AnimatedGameComponents;

// 171 * 32 minimum, actual 224 * 56
internal class Label : AnimatedGameComponent
{
    /// <summary>
    /// Vertical offset from the player area position.
    /// </summary>
    static readonly int OffsetY = 140;

    /// <summary>
    /// Distance from the label texture boundary for the text background.
    /// </summary>
    static readonly int TextBackgroundMarging = 5;

    static readonly TimeSpan AnimationDuration = TimeSpan.FromMilliseconds(400);

    /// <summary>
    /// Dictionary of label colors that correlate to players states.
    /// </summary>
    static readonly Dictionary<PlayerState, Color> Colors = new()
    {
        { PlayerState.Raised, Color.LightGreen } // ddf9e7
    };

    public static readonly Dictionary<PlayerState, string> Descriptions = new()
    {
        { PlayerState.Checked, "Check" },
        { PlayerState.Folded, "Fold" },
        { PlayerState.Raised, "Raise" },
        { PlayerState.Called, "Call" },
        { PlayerState.AllIn, "All In" }
    };

    /// <summary>
    /// Texture used as a background for the text.
    /// </summary>
    readonly Texture2D _textBackground;

    /// <summary>
    /// Player that this label is attached to.
    /// </summary>
    readonly PokerBettingPlayer _player;

    readonly Vector2 _hiddenPosition;
    readonly Vector2 _visiblePosition;

    public Label(PokerBettingPlayer player, GameManager gm) : base(gm, Art.Label)
    {
        _player = player;
        _player.StateChanged += Player_OnStateChanged;

        // create text background
        int width = Texture.Width - TextBackgroundMarging * 2;
        int height = Texture.Height - TextBackgroundMarging * 2;
        _textBackground = new Texture2D(Game.GraphicsDevice, width, height);
        FillTextureBackground(Color.White);

        // calculate visible position
        float x = _player.IsOnTheLeft ?
            _player.Position.X + Constants.PlayerAreaWidth - Texture.Width :
            _player.Position.X;
        float y = _player.IsAtTheBottom ?
            _player.Position.Y - OffsetY - Texture.Height :
            _player.Position.Y + Constants.CardSize.Y + OffsetY;
        _visiblePosition = new Vector2(x, y);

        // calculate hidden position
        x = _player.IsOnTheLeft ? -Texture.Width : Constants.GameWidth;
        _hiddenPosition = new Vector2(x, _visiblePosition.Y);

        Position = _hiddenPosition;
        //TextColor = Color.White;

        Hide();
    }

    public override void Draw(GameTime gameTime)
    {
        var gm = Game.Services.GetService<GameManager>();
        var sb = Game.Services.GetService<SpriteBatch>();
        sb.Begin();

        int x = (int)Position.X + TextBackgroundMarging;
        int y = (int)Position.Y + TextBackgroundMarging;
        var dest = new Rectangle(x, y, _textBackground.Width, _textBackground.Height);
        sb.Draw(_textBackground, dest, Color.LightBlue);

        sb.End();
        base.Draw(gameTime);
    }

    protected override void DrawText(string text, SpriteFont font, Rectangle destination, Vector2 position)
    {
        //font = Fonts.Moire.Bold;
        var sb = Game.Services.GetService<SpriteBatch>();

        // measure text
        Vector2 size = font.MeasureString(Text);
        var playerTextPos = _player.GetCenteredTextPosition(text, 3);

        // calculate text position
        float offsetX = _visiblePosition.X - Position.X;
        float x = playerTextPos.X - offsetX;
        float y = position.Y + (destination.Height - size.Y) / 2;
        Vector2 textPosition = new(x, y);

        // draw text
        sb.DrawString(font, text, textPosition, TextColor);
    }

    void FillTextureBackground(Color color)
    {
        int width = _textBackground.Width;
        int height = _textBackground.Height;
        var data = new Color[width * height];

        for (int i = 0; i < data.Length; i++)
            data[i] = color;

        _textBackground.SetData(data);
    }

    public void Show()
    {
        Position = _hiddenPosition;
        Enabled = true;
        Visible = true;
    }

    public void Hide()
    {
        RemoveAnimations();
        Visible = false;
        Enabled = false;
    }

    void Extend()
    {
        RemoveAnimations();
        AddAnimation(new TransitionGameComponentAnimation(Position, _visiblePosition)
        {
            Duration = AnimationDuration,
        });
    }

    void Retract(Action<object> performWhenDone = null)
    {
        RemoveAnimations();
        var anim = new TransitionGameComponentAnimation(Position, _hiddenPosition)
        {
            Duration = AnimationDuration,
        };
        if (performWhenDone != null)
        {
            anim.PerformWhenDone = performWhenDone;
        }
        AddAnimation(anim);
    }

    void Player_OnStateChanged(object o, PlayerStateChangedEventArgs e)
    {
        if (e.PrevState == PlayerState.Waiting)
        {
            Text = Descriptions[e.NewState];
            Extend();
        }
        else if (e.NewState == PlayerState.Waiting)
        {
            Retract();
        }
        else
        {
            void performWhenDone(object o)
            {
                Text = Descriptions[e.NewState];
                Extend();
            }
            Retract(performWhenDone);
        }
    }
}