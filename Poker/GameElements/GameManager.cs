using Framework.Engine;
using Framework.Misc;
using System;

namespace Poker.GameElements;

class GameManager : CardGame
{
    readonly CardPile _cardPile;

    public GameManager(Game game) : base(1, 0, CardSuits.AllSuits, CardValues.NonJokers,
        Constants.MinPlayers, Constants.MaxPlayers, new PokerTable(game), Constants.DefaultTheme, game)
    {
        _cardPile = new CardPile(this);
        game.Components.Add(_cardPile);

        // create players
        AddPlayer(new HumanPlayer(GetRandomName(), this));
        for (int i = 0; i < MaximumPlayers - 1; i++)
            AddPlayer(new AIPlayer(GetRandomName(), this));
    }

    /// <summary>
    /// Selects a random, unique name of an alternating gender.
    /// </summary>
    /// <returns></returns>
    string GetRandomName()
    {
        string name;

        // alternate gender
        Gender gender = (Gender)(Players.Count % 2);

        // get the count of 50% of the names (first half is male, second female)
        int nameCount = Constants.Names.Length / 2;
        do
        {
            // get random index taking into consideration the appropriate half of collection
            int offset = gender == Gender.Male ? 0 : nameCount;
            int index = ((PokerGame)Game).Random.Next(0, nameCount);

            // retrieve the name and cap index (just in case)
            name = Constants.Names[Math.Min(index + offset, Constants.Names.Length - 1)];

            // repeat until a unique name is selected
        } while(Players.Find(p => p.Name == name) is PokerPlayer);

        return name;
    }

    /// <summary>
    /// Changes card theme for the game.
    /// </summary>
    /// <param name="theme">New card theme.</param>
    public void SetTheme(string theme)
    {
        if (Theme == theme) return;
        else if (theme != Constants.RedThemeText && theme != Constants.BlueThemeText) return;
        else
        {
            Theme = theme;
            OnThemeChanged(theme);
        }
    }

    public override void StartPlaying()
    {
        _cardPile.PlayShuffleAnimation();
    }

    /// <summary>
    /// Adds a player to the game.
    /// </summary>
    /// <param name="player">The player to add.</param>
    public override void AddPlayer(Player player)
    {
        if (player is PokerPlayer && Players.Count < MaximumPlayers)
            Players.Add(player);
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

    void OnThemeChanged(string theme)
    {
        ThemeChanged?.Invoke(this, new ThemeChangedEventArgs(theme));
    }

    public event EventHandler<ThemeChangedEventArgs> ThemeChanged;
}