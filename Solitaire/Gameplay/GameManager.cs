using Framework.Assets;
using Framework.Engine;
using Framework.Misc;
using Solitaire.Misc;
using Solitaire.UI;

namespace Solitaire.Gameplay;

internal class GameManager : CardGame
{
    public GameManager(Game game) : base(1, 0, CardSuits.AllSuits, CardValues.NonJokers, Fonts.Moire.Regular,
        1, 1, new SolitaireTable(game), Strings.Red, game)
    {

    }

    public override void AddPlayer(Player player)
    {
        throw new NotImplementedException();
    }

    public override void DealCardsToPlayers()
    {
        throw new NotImplementedException();
    }

    public override Player GetCurrentPlayer()
    {
        throw new NotImplementedException();
    }

    public override void StartPlaying()
    {
        throw new NotImplementedException();
    }
}