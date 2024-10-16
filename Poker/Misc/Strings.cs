using System.Collections.Generic;

namespace Poker.Misc;

static class Strings
{
    public static readonly string[] Names = new[] {
        "Liam", "Noah", "Oliver", "James", "Elijah", "Theodore", "Henry", "Lucas", "William",
        "Olivia", "Emily", "Charlotte", "Amelia", "Sophia", "Mia", "Isabella", "Ava", "Evelyn"};

    public static readonly string Check = "Check";
    public static readonly string Fold = "Fold";
    public static readonly string Raise = "Raise";
    public static readonly string Call = "Call";
    public static readonly string AllIn = "All In";
    public static readonly string Bankrupt= "Bankrupt";
    public static readonly string Winner = "Winner";

    public static readonly string Clear = "Clear";
    public static readonly string Shuffle = "Shuffle";
    public static readonly string Restart = "Restart";
    public static readonly string Deal = "Deal";
    public static readonly string Exit = "Exit";
    public static readonly string Continue = "Continue";

    public static readonly string Red = "Red";
    public static readonly string Blue = "Blue";

    public static readonly string Better = "Better";
    public static readonly string Worse = "Worse";
    public static readonly string Equal = "Equal";
    public static readonly string Card = "Card";

    public static readonly string Play = "Play";
    public static readonly string Theme = "Theme";
    public static readonly string Test = "Test";

    public static readonly string CardArrayLengthException =
        "Cards array must have the length of 5.";
}