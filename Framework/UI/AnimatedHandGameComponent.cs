using Framework.Engine;
using Framework.Misc;
using Microsoft.Xna.Framework;

namespace Framework.UI;

public class AnimatedHandGameComponent : AnimatedGameComponent
{
    public readonly Hand Hand;
    readonly List<AnimatedCardGameComponent> _animatedCardsHeld = new();

    public override bool IsAnimating
    {
        get
        {
            for (int animationIndex = 0; animationIndex < _animatedCardsHeld.Count; animationIndex++)
                if (_animatedCardsHeld[animationIndex].IsAnimating)
                    return true;
            return false;
        }
    }

    /// <summary>
    /// Returns the animated cards contained in the hand.
    /// </summary>
    public IEnumerable<AnimatedCardGameComponent> AnimatedCards =>
        _animatedCardsHeld.AsReadOnly();

    /// <summary>
    /// Initializes a new instance of the animated hand component. This means
    /// setting the hand's position and initializing all animated cards and their
    /// respective positions. Also, registrations are performed to the associated
    /// <paramref name="hand"/> events to update the animated hand as cards are
    /// added or removed.
    /// </summary>
    /// <param name="place">The player's place index (-1 for the dealer).</param>
    /// <param name="hand">The hand represented by this instance.</param>
    /// <param name="cardGame">The associated card game.</param>
    public AnimatedHandGameComponent(int place, Hand hand, CardGame cardGame) : base(cardGame, null)
    {
        Hand = hand;
        hand.CardReceived += Hand_OnCardReceived;
        hand.CardRemoved += Hand_OnCardRemoved;

        // Set the component's position
        Position = place == -1 ? cardGame.GameTable.DealerPosition :
            cardGame.GameTable[place];

        // Create and initialize animated cards according to the cards in the associated hand
        for (int cardIndex = 0; cardIndex < hand.Count; cardIndex++)
        {
            AnimatedCardGameComponent animatedCardGameComponent = new(hand[cardIndex], cardGame)
            {
                Position = Position + new Vector2(30 * cardIndex, 0)
            };
            _animatedCardsHeld.Add(animatedCardGameComponent);
            Game.Components.Add(animatedCardGameComponent);
        }

        Game.Components.ComponentRemoved += Components_OnComponentRemoved;
    }

    /// <summary>
    /// Updates the component.
    /// </summary>
    /// <param name="gameTime">The time which elapsed since this method was last
    /// called.</param>
    public override void Update(GameTime gameTime)
    {
        // Arrange the hand's animated cards' positions
        for (int animationIndex = 0; animationIndex < _animatedCardsHeld.Count; animationIndex++)
            if (!_animatedCardsHeld[animationIndex].IsAnimating)
                _animatedCardsHeld[animationIndex].Position = Position +
                    GetCardRelativePosition(animationIndex);

        base.Update(gameTime);
    }

    /// <summary>
    /// Gets the card's offset from the hand position according to its index
    /// in the hand.
    /// </summary>
    /// <param name="cardLocationInHand">The card index in the hand.</param>
    /// <returns></returns>
    public virtual Vector2 GetCardRelativePosition(int cardLocationInHand) =>
        default;

    /// <summary>
    /// Finds the index of a specified card in the hand.
    /// </summary>
    /// <param name="card">The card to locate.</param>
    /// <returns>The card's index inside the hand, or -1 if it cannot be
    /// found.</returns>
    public int GetCardLocationInHand(TraditionalCard card)
    {
        for (int animationIndex = 0; animationIndex < _animatedCardsHeld.Count; animationIndex++)
            if (_animatedCardsHeld[animationIndex].Card == card)
                return animationIndex;
        return -1;
    }

    /// <summary>
    /// Gets the animated game component associated with a specified card.
    /// </summary>
    /// <param name="card">The card for which to get the animation 
    /// component.</param>
    /// <returns>The card's animation component, or null if such a card cannot
    /// be found in the hand.</returns>
    public AnimatedCardGameComponent GetCardGameComponent(TraditionalCard card)
    {
        int location = GetCardLocationInHand(card);
        if (location == -1)
            return null;

        return _animatedCardsHeld[location];
    }

    /// <summary>
    /// Gets the animated game component associated with a specified card.
    /// </summary>
    /// <param name="location">The location where the desired card is 
    /// in the hand.</param>
    /// <returns>The card's animation component.</return>s 
    public AnimatedCardGameComponent GetCardGameComponent(int location)
    {
        if (location == -1 || location >= _animatedCardsHeld.Count)
            return null;

        return _animatedCardsHeld[location];
    }

    /// <summary>
    /// Handles the ComponentRemoved event of the Components control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The 
    /// <see cref="GameComponentCollectionEventArgs"/> 
    /// instance containing the event data.</param>
    void Components_OnComponentRemoved(object sender, GameComponentCollectionEventArgs e)
    {
        if (e.GameComponent == this)
            Dispose();
    }

    /// <summary>
    /// Handles the hand's <see cref="CardPacket.CardRemoved"/> event be removing the corresponding animated
    /// card.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The 
    /// <see cref="CardEventArgs"/> 
    /// instance containing the event data.</param>
    void Hand_OnCardRemoved(object sender, CardEventArgs e)
    {
        // Remove the card from screen
        for (int animationIndex = 0; animationIndex < _animatedCardsHeld.Count; animationIndex++)
        {
            if (_animatedCardsHeld[animationIndex].Card == e.Card)
            {
                Game.Components.Remove(_animatedCardsHeld[animationIndex]);
                _animatedCardsHeld.RemoveAt(animationIndex);
                return;
            }
        }
    }

    /// <summary>
    /// Handles the <see cref="Hand.CardReceived"/> event be adding a corresponding 
    /// animated card.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The 
    /// <see cref="CardEventArgs"/> 
    /// instance containing the event data.</param>
    void Hand_OnCardReceived(object sender, CardEventArgs e)
    {
        AnimatedCardGameComponent animatedCardGameComponent = new(e.Card, CardGame) { Visible = false };
        _animatedCardsHeld.Add(animatedCardGameComponent);
        Game.Components.Add(animatedCardGameComponent);
    }

    /// <summary>
    /// Calculate the estimated time at which the longest lasting animation currently managed 
    /// will complete.
    /// </summary>
    /// <returns>The estimated time for animation complete </returns>
    public override TimeSpan EstimatedTimeForAnimationsCompletion()
    {
        TimeSpan result = TimeSpan.Zero;

        if (IsAnimating)
        {
            for (int animationIndex = 0; animationIndex < _animatedCardsHeld.Count; animationIndex++)
            {
                if (_animatedCardsHeld[animationIndex].EstimatedTimeForAnimationsCompletion() > result)
                {
                    result = _animatedCardsHeld[animationIndex].EstimatedTimeForAnimationsCompletion();
                }
            }
        }

        return result;
    }

    /// <summary>
    /// Properly disposes of the component when it is removed.
    /// </summary>
    /// <param name="disposing"></param>
    protected override void Dispose(bool disposing)
    {
        // Remove the registrations to the event to make this 
        // instance collectable by gc
        Hand.CardReceived -= Hand_OnCardReceived;
        Hand.CardRemoved -= Hand_OnCardRemoved;

        base.Dispose(disposing);
    }
}