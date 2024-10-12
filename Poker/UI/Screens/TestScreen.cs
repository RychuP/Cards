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
    Dealer _dealer;
    PokerBettingPlayer _player;
    CommunityCards _communityCards;
    GameManager _gameManager;
    Vector2 _playerGamePosition;
    Vector2 _playerTemporaryPosition;
    PokerHand _pokerHand;
    TraditionalCard[] _evaluationResult;

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

        base.Update(gameTime);
    }

    public override void Draw(GameTime gameTime)
    {
        var sb = Game.Services.GetService<SpriteBatch>();
        sb.Begin();

        string text = _pokerHand != PokerHand.HighCard ? 
            $"Poker hand: {_pokerHand}" : 
            "No poker hands in this draw.";
        var size = _gameManager.Font.MeasureString(text);
        float x = (Constants.GameWidth - size.X) / 2;
        float y = (int)_communityCards.AnimatedHand.Position.Y - size.Y * 3;
        sb.DrawString(_gameManager.Font, text, new Vector2(x, y), Color.WhiteSmoke);

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