using Framework.Misc;

namespace Framework.Engine;

/// <summary>
/// Traditional-western card
/// </summary>
/// <remarks>
/// Each card has a defined <see cref="CardSuits">Type</see> and <see cref="CardValues">Value</see>
/// as well as the <see cref="Engine.CardPacket"/> in which it is being held.
/// A card may not be held in more than one <see cref="Engine.CardPacket"/>. This is achived by enforcing any card transfer
/// operation between <see cref="CarkPacket"/>s and <see cref="Hand"/>s to be performed only from within the card's 
/// <see cref="MoveToHand"/> method only. This method accesses <c>internal</c> <see cref="Hand.Add"/> method and 
/// <see cref="CardPacket.Remove"/> method accordingly to complete the card transfer operation.
/// </remarks>
public class TraditionalCard : IComparable<TraditionalCard>
{
    public static readonly int Width = 80;
    public static readonly int Height = 106;

    /// <summary>
    /// Whether the ace should be considered lowest or highest value.
    /// </summary>
    public static bool AceIsHighestValue { get; set; } = true;
    public CardSuits Type { get; set; }
    public CardValues Value { get; set; }
    public CardPacket CardPacket { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TraditionalCard"/> class.
    /// </summary>
    /// <param name="type">The card suit. Supports only a single value.</param>
    /// <param name="value">The card's value. Only single values are 
    /// supported.</param>
    /// <param name="holdingCardCollection">The holding card collection.</param>
    internal TraditionalCard(CardSuits type, CardValues value, CardPacket holdingCardCollection)
    {
        // Check for single type
        switch (type)
        {
            case CardSuits.Club:
            case CardSuits.Diamond:
            case CardSuits.Heart:
            case CardSuits.Spade:
                break;
            default:
                throw new ArgumentException("type must be a single value", nameof(type));
        }

        // Check for single value
        switch (value)
        {
            case CardValues.Ace:
            case CardValues.Two:
            case CardValues.Three:
            case CardValues.Four:
            case CardValues.Five:
            case CardValues.Six:
            case CardValues.Seven:
            case CardValues.Eight:
            case CardValues.Nine:
            case CardValues.Ten:
            case CardValues.Jack:
            case CardValues.Queen:
            case CardValues.King:
            case CardValues.FirstJoker:
            case CardValues.SecondJoker:
                break;
            default:
                throw new ArgumentException("value must be single value", nameof(value));
        }

        Type = type;
        Value = value;
        CardPacket = holdingCardCollection;
    }

    /// <summary>
    /// Moves the card from its current <see cref="Engine.CardPacket"/> 
    /// to the specified <paramref name="hand"/>. 
    /// This method of operation prevents any one card instance from being held by more than one
    /// <see cref="Engine.CardPacket"/> at the same time.
    /// </summary>
    /// <param name="hand">The receiving hand.</param>
    public void MoveToHand(Hand hand)
    {
        CardPacket.Remove(this);
        CardPacket = hand;
        hand.Add(this);
    }

    public int CompareTo(TraditionalCard other)
    {
        if (Value == other.Value)
        {
            return 0;
        }
        else
        {
            int val1 = (Value == CardValues.Ace && AceIsHighestValue) ? 
                (int)CardValues.King + 1 : (int)Value;
            int val2 = (other.Value == CardValues.Ace && AceIsHighestValue) ? 
                (int)CardValues.King + 1 : (int)other.Value;

            if (val1 > val2)
            {
                return -1;
            }
            else
            {
                return 1;
            }
        }
    }
}