using CardsFramework;
using System;

namespace Poker.GameElements;

class GameManager : CardGame
{
    readonly CardPile _cardPile;

    public GameManager(Game game) : base(1, 0, CardSuits.AllSuits, CardValues.NonJokers,
        Constants.MinPlayers, Constants.MaxPlayers, new PokerTable(game), Constants.DefaultTheme, game)
    {
        SpriteBatch = Art.SpriteBatch;
        _cardPile = new CardPile(this);
        game.Components.Add(_cardPile);
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