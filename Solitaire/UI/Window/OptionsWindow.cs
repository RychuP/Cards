using Framework.Assets;
using Framework.Engine;
using Microsoft.Xna.Framework.Graphics;
using Solitaire.Gameplay.Piles;
using Solitaire.Managers;
using Solitaire.UI.Screens;

namespace Solitaire.UI.Window;

internal class OptionsWindow : Window
{
    TextLine _difficultyDesc => TextLines[0];
    Texture2D[] _faceUpCards = new Texture2D[3];
    Vector2 _stockPosition;

    public OptionsWindow(Vector2 position, Rectangle bounds, OptionsScreen optionsScreen) 
        : base(position, bounds, optionsScreen)
    {
        AddText(GetDifficultyDescription());
        UpdateTextAndPositions();

        _faceUpCards[0] = CardAssets.CardFaces["ClubQueen"];
        _faceUpCards[1] = CardAssets.CardFaces["HeartJack"];
        _faceUpCards[2] = CardAssets.CardFaces["SpadeAce"];

        GameManager.DifficultyChanged += (o, e) => UpdateTextAndPositions();
    }

    public override void Draw(GameTime gameTime)
    {
        var sb = Game.Services.GetService<SpriteBatch>();
        sb.Begin();

        // draw difficulty description
        sb.DrawString(GameManager.Font, _difficultyDesc.Text, _difficultyDesc.Position, 
            Color.WhiteSmoke * Opacity);

        // draw stock
        var cardBack = CardAssets.CardBacks[GameManager.Theme];
        sb.Draw(cardBack, _stockPosition, Color.White * Opacity);

        // draw face up cards
        var pos = _stockPosition;
        pos += new Vector2(TraditionalCard.Width + Pile.CardSpacing.X, 0);
        for (int i = 0; i < GameManager.GetDrawCount(); i++)
        {
            sb.Draw(_faceUpCards[i], pos, Color.White * Opacity);
            pos += new Vector2(Pile.CardSpacing.X, 0);
        }

        sb.End();
    }

    void UpdateTextAndPositions()
    {
        // update text
        _difficultyDesc.Text = GetDifficultyDescription();

        // calculate stock position
        int faceUpCardCount = GameManager.GetDrawCount();
        int faceUpCardsWidth = TraditionalCard.Width + Pile.CardSpacing.X * (faceUpCardCount - 1);
        int totalCardsWidth = TraditionalCard.Width + Pile.CardSpacing.X + faceUpCardsWidth;
        float x = Position.X + (Size.X - totalCardsWidth) / 2;
        float y = _difficultyDesc.Position.Y + _difficultyDesc.Size.Y + 40;
        _stockPosition = new Vector2(x, y);
    }

    string GetDifficultyDescription()
    {
        int count = GameManager.GetDrawCount();
        string s = count > 1 ? "s" : string.Empty;
        return $"Draw {count} card{s}";
    }
}