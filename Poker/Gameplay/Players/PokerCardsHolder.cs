using Framework.Assets;
using Framework.Engine;
using Framework.UI;
using System;

namespace Poker.Gameplay.Players;

abstract class PokerCardsHolder : Player
{
    public AnimatedHandGameComponent AnimatedHand { get; init; }
    protected Game Game => CardGame.Game;
    protected GameManager GameManager => CardGame as GameManager;

    public PokerCardsHolder(string name, GameManager gm) : base(name, gm)
    { }

    /// <summary>
    /// Puts everything in the default state.
    /// </summary>
    public virtual void Reset()
    {
        if (Hand.Count > 0)
            ReturnCardsToDealer();
    }

    /// <summary>
    /// Display an animation when a card is dealt.
    /// </summary>
    /// <param name="card">The card being dealt.</param>
    /// <param name="flipCard">Should the card be flipped after dealing it.</param>
    /// <param name="startTime">The time at which the animation should start.</param>
    public void AddDealAnimation(TraditionalCard card, bool flipCard, DateTime startTime)
    {
        // Get the card location and card component
        int cardLocationInHand = AnimatedHand.GetCardLocationInHand(card);
        AnimatedCardGameComponent cardComponent = AnimatedHand.GetCardGameComponent(cardLocationInHand);

        // Add the transition animation
        cardComponent.AddAnimation(
            new TransitionGameComponentAnimation(Constants.DealerPosition,
            AnimatedHand.Position + AnimatedHand.GetCardRelativePosition(cardLocationInHand))
            {
                StartTime = startTime,
                PerformBeforeStart = (o) => cardComponent.Visible = true,
                PerformWhenDone = (o) => CardSounds.Deal.Play(),
                Duration = Constants.DealAnimationDuration
            });

        if (flipCard)
        {
            // Add the flip animation
            cardComponent.AddAnimation(new FlipGameComponentAnimation
            {
                IsFromFaceDownToFaceUp = true,
                Duration = Constants.CardFlipAnimationDuration,
                StartTime = startTime + Constants.DealAnimationDuration,
                PerformWhenDone = (o) => CardSounds.Flip.Play()
            });
        }
    }

    /// <summary>
    /// Gets the player hand positions according to the player index.
    /// </summary>
    /// <param name="playerIndex">The player's index.</param>
    /// <returns>The position for the player's hand on the game table.</returns>
    public static Vector2 GetPlayerPosition(int playerIndex)
    {
        return playerIndex switch
        {
            0 => new Vector2(Constants.PlayerCardMargin, Constants.GameHeight - Constants.CardSize.Y - Constants.PlayerCardMargin),
            1 => new Vector2(Constants.PlayerCardMargin, Constants.PlayerCardMargin),
            2 => new Vector2(Constants.GameWidth - Constants.CardSize.X * 2 - Constants.PlayerCardMargin - Constants.PlayerCardPadding,
                Constants.PlayerCardMargin),
            3 => new Vector2(Constants.GameWidth - Constants.CardSize.X * 2 - Constants.PlayerCardMargin - Constants.PlayerCardPadding,
                Constants.GameHeight - Constants.CardSize.Y - Constants.PlayerCardMargin),
            _ => throw new ArgumentException("Player index should be between 0 and 3", nameof(playerIndex)),
        };
    }

    public void ReturnCardsToDealer()
    {
        Hand.DealCardsToHand(GameManager.Dealer, Hand.Count);
    }

    public bool HasCard(TraditionalCard card)
    {
        for (int i = 0; i < Hand.Count; i++)
            if (Hand[i] == card) return true;
        return false;
    }

    public void RaiseCard(TraditionalCard card, DateTime startTime)
    {
        if (!HasCard(card))
            throw new ArgumentException("No such card in hand.", nameof(card));

        var animatedCard = AnimatedHand.GetCardGameComponent(card);
        if (animatedCard is null)
            throw new Exception("Internal error. " +
                    $"The card is missing its animated component.");

        var dest = animatedCard.Position - new Vector2(0, 30);
        animatedCard.AddAnimation(new TransitionGameComponentAnimation(animatedCard.Position, dest)
        {
            StartTime = startTime,
            PerformWhenDone = (o) => CardSounds.Deal.Play(),
            Duration = TimeSpan.FromMilliseconds(200)
        });
    }
}