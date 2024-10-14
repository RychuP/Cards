using Framework.Engine;
using Microsoft.Xna.Framework.Graphics;
using Poker.Gameplay;
using Poker.Gameplay.Evaluation;
using Poker.Gameplay.Players;
using Poker.UI.BaseScreens;
using System;

namespace Poker.UI.Screens;

internal class TestScreen : StaticGameScreen
{
    const string BetterText = "Better";
    const string WorseText = "Worse";
    const string EqualText = "Equal";
    const string CardText = "Card";
    readonly Color TextColor = Color.WhiteSmoke;

    /// <summary>
    /// Delay after which to display the hand and card description texts.
    /// </summary>
    readonly TimeSpan TextDelay = Constants.DealAnimationDuration + Constants.CardFlipAnimationDuration;

    /// <summary>
    /// Distance of the better/worse card description from the card.
    /// </summary>
    const int TextOffsetX = 50;

    Dealer _dealer;
    PokerBettingPlayer _player;
    CommunityCards _communityCards;
    GameManager _gameManager;
    Vector2 _playerGamePosition;
    Vector2 _playerTemporaryPosition;
    PokerHand _pokerHand;
    TraditionalCard[] _evaluationResult;

    /// <summary>
    /// Cards finished their transition and flip animations.
    /// </summary>
    bool _cardsAreDown;

    /// <summary>
    /// Measures time from the start of the deal animation.<br></br> 
    /// It's compared with <see cref="TextDelay"/>.
    /// </summary>
    TimeSpan _textShowingDelay;

    public TestScreen(ScreenManager sm) : base(sm, 2)
    { }

    public override void Initialize()
    {
        AddButton("Shuffle", ShuffleButton_OnClick);
        AddButton("Exit", ExitButton_OnClick);

        _gameManager = Game.Services.GetService<GameManager>();
        _communityCards = _gameManager.CommunityCards;
        _dealer = _gameManager.GetPokerDealer();
        _player = _gameManager[0];
        _playerGamePosition = _player.AnimatedHand.Position;
        int x = (Constants.GameWidth - Constants.PlayerAreaWidth) / 2;
        var y = Constants.CommunityCardsPosition.Y + Constants.CardSize.Y + 50;
        _playerTemporaryPosition = new Vector2(x, y);

        base.Initialize();
    }

    protected override void LoadContent()
    {
        Texture = Art.TableCardOutlines;
        base.LoadContent();
    }

    public override void Update(GameTime gameTime)
    {
        // raise winning cards
        if (_pokerHand != PokerHand.HighCard && !_communityCards.AnimatedHand.IsAnimating)
        {
            if (_evaluationResult != null)
            {
                DateTime startTime = DateTime.Now;

                foreach (var card in _evaluationResult)
                {
                    if (_communityCards.HasCard(card))
                    {
                        _communityCards.RaiseCard(card, startTime);
                        startTime += TimeSpan.FromMilliseconds(200);
                    }
                    else if (_player.HasCard(card))
                    {
                        _player.RaiseCard(card, startTime);
                        startTime += TimeSpan.FromMilliseconds(200);
                    }
                }

                _evaluationResult = null;
            }
        }

        // check when all the cards are down and their transition and flip animations are done
        if (!_cardsAreDown)
        {
            _textShowingDelay += gameTime.ElapsedGameTime;
            if (_textShowingDelay >= TextDelay)
                _cardsAreDown = true;
        }

        base.Update(gameTime);
    }

    public override void Draw(GameTime gameTime)
    {
        if (!_cardsAreDown)
            return;

        var sb = Game.Services.GetService<SpriteBatch>();
        sb.Begin();

        // draw hand description
        string text = _pokerHand != PokerHand.HighCard ? 
            $"Poker hand: {Evaluator.PokerHands[_pokerHand]}" : 
            "No poker hands in this draw.";
        var cardTextSize = _gameManager.Font.MeasureString(text);
        float x = (Constants.GameWidth - cardTextSize.X) / 2;
        float y = (int)_communityCards.AnimatedHand.Position.Y - cardTextSize.Y * 3;
        sb.DrawString(_gameManager.Font, text, new Vector2(x, y), TextColor);

        // determine card description text
        string leftCardDesc = EqualText,
            rightCardDesc = EqualText;
        if (_player.BetterCard.Value != _player.WorseCard.Value)
        {
            leftCardDesc = _player.BetterCard == _player.Hand[0] ?
                BetterText : WorseText;
            rightCardDesc = _player.BetterCard == _player.Hand[1] ?
                BetterText : WorseText;
        }

        // measure card text size
        cardTextSize = _gameManager.Font.MeasureString(CardText);
        float textHeight = cardTextSize.Y * 2 + Constants.TextVerticalSpacing;

        // calculate left card text position
        x = _playerTemporaryPosition.X - TextOffsetX - cardTextSize.X;
        y = _playerTemporaryPosition.Y + ((Constants.CardSize.Y - textHeight) / 2) + 
            cardTextSize.Y + Constants.TextVerticalSpacing;
        Vector2 leftCardTextPos = new(x, y);

        // measure left card desc size
        var leftDescSize = _gameManager.Font.MeasureString(leftCardDesc);

        // calculate left card desc position
        float offsetX = (leftDescSize.X - cardTextSize.X) / 2;
        x = leftCardTextPos.X - offsetX;
        y = leftCardTextPos.Y - Constants.TextVerticalSpacing - leftDescSize.Y;
        Vector2 leftCardDescPos = new(x, y);

        // measure right card desc size
        var rightDescSize = _gameManager.Font.MeasureString(rightCardDesc);

        // calculate right card text position
        x = _playerTemporaryPosition.X + Constants.PlayerAreaWidth + TextOffsetX;
        Vector2 rightCardTextPos = new(x, leftCardTextPos.Y);

        // calculate right card desc position
        offsetX = (rightDescSize.X - cardTextSize.X) / 2;
        x = rightCardTextPos.X - offsetX;
        Vector2 rightCardDescPos = new(x, leftCardDescPos.Y);

        // draw player card descriptions
        if (leftCardDesc == BetterText || leftCardDesc == EqualText)
        {
            sb.DrawString(_gameManager.Font, leftCardDesc, leftCardDescPos, TextColor);
            sb.DrawString(_gameManager.Font, CardText, leftCardTextPos, TextColor);
        }
        if (rightCardDesc == BetterText || rightCardDesc == EqualText)
        {
            sb.DrawString(_gameManager.Font, rightCardDesc, rightCardDescPos, TextColor);
            sb.DrawString(_gameManager.Font, CardText, rightCardTextPos, TextColor);
        }

        sb.End();
        base.Draw(gameTime);
    }

    void Deal()
    {
        _dealer.Shuffle();

        // deal player cards
        for (int dealIndex = 0; dealIndex < 2; dealIndex++)
        {
            TraditionalCard card = _dealer.DealCardToHand(_player.Hand);
            _player.AddDealAnimation(card, true, DateTime.Now);
        }

        // deal community cards 
        for (int i = 0; i < 5; i++)
        {
            TraditionalCard card = _dealer.DealCardToHand(_communityCards.Hand);
            _communityCards.AddDealAnimation(card, true, DateTime.Now);
        }

        // check result
        _pokerHand = Evaluator.CheckHand(_player, _communityCards, out _evaluationResult);

        _cardsAreDown = false;
        _textShowingDelay = TimeSpan.Zero;
    }

    void ReturnCards()
    {
        _communityCards.ReturnCardsToDealer();
        _player.ReturnCardsToDealer();
    }

    void ShuffleButton_OnClick(object o, EventArgs e)
    {
        ReturnCards();
        Deal();
    }

    void ExitButton_OnClick(object o, EventArgs e) =>
        _gameManager.StopPlaying();

    protected override void OnVisibleChanged(object sender, EventArgs args)
    {
        if (Visible == true)
        {
            _player.AnimatedHand.Position = _playerTemporaryPosition;
            _gameManager.Reset();
            _pokerHand = PokerHand.HighCard;
            _evaluationResult = null;
            _dealer.ShufflingType = ShufflingType.Ordered;
            Deal();
        }
        else
        {
            _player.AnimatedHand.Position = _playerGamePosition;
            _dealer.ShufflingType = ShufflingType.Random;
            ReturnCards();
        }

        base.OnVisibleChanged(sender, args);
    }
}