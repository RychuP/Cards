using Framework.Engine;
using Framework.Misc;
using Solitaire.Gameplay.Piles;
using System.Linq;

namespace Solitaire.UI.AnimatedPiles;

internal class AnimatedTableau : AnimatedPile
{
    /// <summary>
    /// Size of the visible part of a card when partially covered by other cards in a stack.
    /// </summary>
    static readonly Point ReducedCardSize = new(TraditionalCard.Width, Pile.CardSpacing.Y);

    public AnimatedTableau(Tableau tableau) : base(tableau)
    {
        Hand.CardRemoved += Hand_OnCardRemoved;
        GameManager.GameStart += GameManager_OnGameStart;
    }

    // tableau cards stack down with gaps between each card
    public override Vector2 GetCardRelativePosition(int cardLocationInHand) =>
        new(0, Pile.CardSpacing.Y * cardLocationInHand);

    /// <inheritdoc/>
    /// <remarks>Only face up cards are taken into consideration.</remarks>
    public override TraditionalCard GetCardFromPosition(Point position)
    {
        var lastAnimCard = AnimatedCards.Last();
        for (int i = 0; i < Hand.Count; i++)
        {
            var card = Hand[i];
            var animCard = GetCardGameComponent(card);
            var size = animCard == lastAnimCard ? TraditionalCard.Size : ReducedCardSize;
            var bounds = new Rectangle(animCard.Position.ToPoint(), size);

            if (bounds.Contains(position))
            {
                if (!animCard.IsFaceDown)
                    return card;
                else
                    break;
            }
        }
        return null;
    }

    void Hand_OnCardRemoved(object o, CardEventArgs e)
    {
        if (Hand.Count > 0)
        {
            var animCard = AnimatedCards.Last();
            if (animCard.IsFaceDown)
                animCard.IsFaceDown = false;
        }
    }

    protected override void Hand_OnCardReceived(object o, CardEventArgs e)
    {
        base.Hand_OnCardReceived(o, e);
        var animCard = GetCardGameComponent(e.Card);
        animCard.IsFaceDown = false;
    }

    protected virtual void GameManager_OnGameStart(object o, EventArgs e)
    {
        // Sets all the opening cards face down apart from the last one.
        foreach (var animCard in AnimatedCards)
            animCard.IsFaceDown = true;
        var topCard = AnimatedCards.Last();
        topCard.IsFaceDown = false;
    }
}