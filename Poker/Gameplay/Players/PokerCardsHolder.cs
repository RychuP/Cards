using Framework.Assets;
using Framework.Engine;
using Framework.UI;
using System;

namespace Poker.Gameplay.Players;

abstract class PokerCardsHolder : Player
{
    public AnimatedHandGameComponent AnimatedHand { get; init; }

    public PokerCardsHolder(string name, GameManager gm) : base(name, gm)
    {
        
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
                Duration = Constants.FlipAnimationDuration,
                StartTime = startTime + Constants.DealAnimationDuration,
                PerformWhenDone = (o) => CardSounds.Flip.Play()
            });
        }
    }
}