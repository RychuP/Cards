using CardsFramework;
using Microsoft.Xna.Framework;

namespace Poker;

internal class PokerCardGame : CardGame
{
    public PokerCardGame(Game game) : base(1, 0, CardSuit.AllSuits, CardValue.NonJokers, 1, 4, 
        new PokerTable(game), "Red", game)
    { 
    
    }

    public override void StartPlaying()
    {
        throw new System.NotImplementedException();
    }

    public override void AddPlayer(Player player)
    {
        throw new System.NotImplementedException();
    }

    public override void CheckRules()
    {
        base.CheckRules();
    }

    public override void Deal()
    {
        throw new System.NotImplementedException();
    }

    public override Player GetCurrentPlayer()
    {
        throw new System.NotImplementedException();
    }
}