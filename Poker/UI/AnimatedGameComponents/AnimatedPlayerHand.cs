using Framework.Assets;
using Framework.Engine;
using Framework.UI;
using Poker.Gameplay.Players;
using System;

namespace Poker.UI.AnimatedGameComponents;

class AnimatedPlayerHand : AnimatedHandGameComponent
{
    readonly PokerBettingPlayer _player;

    /// <summary>
    /// Creates a new instance of the 
    /// <see cref="AnimatedPlayerHand"/> class.
    /// </summary>
    /// <param name="place">A number indicating the hand's position on the 
    /// game table.</param>
    /// <param name="hand">The player's hand.</param>
    /// <param name="cardGame">The associated game.</param>
    public AnimatedPlayerHand(PokerBettingPlayer player)
        : base(player.Place, player.Hand, player.CardGame)
    { 
        _player = player;
    }

    /// <summary>
    /// Gets the position relative to the hand position at which a specific card
    /// contained in the hand should be rendered.
    /// </summary>
    /// <param name="cardLocationInHand">The card's location in the hand (0 is the
    /// first card in the hand).</param>
    /// <returns>An offset from the hand's location where the card should be 
    /// rendered.</returns>
    public override Vector2 GetCardRelativePosition(int cardLocationInHand)
    {
        int offsetX = (Constants.CardSize.X + Constants.PlayerCardPadding) * cardLocationInHand;
        return new Vector2(offsetX, 0);
    }

    public void Fold()
    {
        for (int i = 0; i < Hand.Count; i++)
        {
            var card = Hand[i];
            var animatedCard = GetCardGameComponent(card) ??
                throw new Exception("Internal error. " +
                    $"The card with index {i} is missing its animated component.");

            var animDuration = TimeSpan.FromMilliseconds(300);

            if (!IsFaceDown)
            {
                animatedCard.AddAnimation(new FlipGameComponentAnimation()
                {
                    Duration = animDuration,
                    IsFromFaceDownToFaceUp = false,
                    PerformWhenDone = (o) => CardSounds.Flip.Play()
                });
            }

            var startPos = Position + GetCardRelativePosition(i);
            int u = 40;
            var offsetY = _player.IsAtTheBottom ? u : -u;
            if ((i == 1 && _player.IsOnTheLeft) || (i == 0 && _player.IsOnTheRight))
                u += Constants.CardSize.X / 2;
            var offsetX = _player.IsOnTheLeft ? -u : u;

            animatedCard.AddAnimation(new TransitionGameComponentAnimation(startPos,
                startPos + new Vector2(offsetX, offsetY))
            {
                Duration = animDuration,
                PerformWhenDone = (o) => CardSounds.Deal.Play()
            });
        }
    }

    public void Flip(DateTime startTime)
    {
        for (int i = 0; i < Hand.Count; i++)
        {
            var card = Hand[i];
            var animatedCard = GetCardGameComponent(card) ??
                throw new Exception("Internal error. " +
                    $"The card with index {i} is missing its animated component.");

            animatedCard.AddAnimation(new FlipGameComponentAnimation()
            {
                IsFromFaceDownToFaceUp = animatedCard.IsFaceDown,
                StartTime = startTime,
                PerformWhenDone = (o) => CardSounds.Flip.Play()
            });
        }
    }
}