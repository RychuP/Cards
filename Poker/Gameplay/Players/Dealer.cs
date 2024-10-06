using Framework.Engine;

namespace Poker.Gameplay.Players;

/// <summary>
/// Card dealer at the poker table.
/// </summary>
/// <remarks>Intended to replace the <see cref="CardGame.Dealer"/>, 
/// to allow getting the cards back from players.</remarks>
class Dealer : Hand
{
    public Dealer(CardPacket cardPacket)
    {
        cardPacket.DealCardsToHand(this, cardPacket.Count);
    }
}