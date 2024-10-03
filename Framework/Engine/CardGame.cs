global using System;
global using System.Collections.Generic;
using Framework.Assets;
using Framework.Misc;
using Framework.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Framework.Engine;

/// <summary>
/// A cards-game handler.
/// </summary>
/// <remarks>
/// Use a singleton of a class that derives from class to empower a cards-game, while making sure
/// to call the various methods in order to allow the implementing instance to run the game.
/// </remarks>
public abstract class CardGame
{
    protected List<GameRule> Rules = new();
    protected List<Player> Players = new();
    protected CardPacket Dealer;
    public int MinimumPlayers { get; protected set; }
    public int MaximumPlayers { get; protected set; }
    public string Theme { get; protected set; }
    public SpriteFont Font { get; init; }
    public GameTable GameTable { get; protected set; }
    public Game Game { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CardGame"/> class.
    /// </summary>
    /// <param name="decks">The amount of decks in the game.</param>
    /// <param name="jokersInDeck">The amount of jokers in each deck.</param>
    /// <param name="suits">The suits which will appear in each deck. Multiple 
    /// values can be supplied using flags.</param>
    /// <param name="cardValues">The card values which will appear in each deck. 
    /// Multiple values can be supplied using flags.</param>
    /// <param name="minimumPlayers">The minimal amount of players 
    /// for the game.</param>
    /// <param name="maximumPlayers">The maximal amount of players 
    /// for the game.</param>
    /// <param name="theme">The name of the theme to use for the 
    /// game's assets.</param>
    /// <param name="game">The associated game object.</param>
    public CardGame(int decks, int jokersInDeck, CardSuits suits, CardValues cardValues, SpriteFont font,
        int minimumPlayers, int maximumPlayers, GameTable gameTable, string theme, Game game)
    {
        Dealer = new CardPacket(decks, jokersInDeck, suits, cardValues);

        Game = game;
        MinimumPlayers = minimumPlayers;
        MaximumPlayers = maximumPlayers;
        Font = font;

        Theme = theme;
        GameTable = gameTable;
        GameTable.DrawOrder = -10000;
        game.Components.Add(GameTable);
    }

    /// <summary>
    /// Initializes static asset classes and adds SpriteBatch service.
    /// </summary>
    /// <param name="game"></param>
    public static void Initialize(Game game)
    {
        game.Services.AddService(typeof(SpriteBatch), new SpriteBatch(game.GraphicsDevice));
        CardAssets.Initialize(game);
        ChipAssets.Initialize(game);
        CardSounds.Initialize(game);
        Fonts.Initialize(game);
    }

    public virtual void Initialize()
    {
        GameTable.Initialize();
    }

    /// <summary>
    /// Returns a card's value in the scope of the game.
    /// </summary>
    /// <param name="card">The card for which to return the value.</param>
    /// <returns>The card's value.</returns>        
    public virtual int GetCardValue(TraditionalCard card)
    {
        return card.Value switch
        {
            CardValues.Ace => 1,
            CardValues.Two => 2,
            CardValues.Three => 3,
            CardValues.Four => 4,
            CardValues.Five => 5,
            CardValues.Six => 6,
            CardValues.Seven => 7,
            CardValues.Eight => 8,
            CardValues.Nine => 9,
            CardValues.Ten => 10,
            CardValues.Jack => 11,
            CardValues.Queen => 12,
            CardValues.King => 13,
            _ => throw new ArgumentException("Ambigous card value"),
        };
    }

    /// <summary>
    /// Checks which of the game's rules need to be fired.
    /// </summary>
    public virtual void CheckRules()
    {
        for (int ruleIndex = 0; ruleIndex < Rules.Count; ruleIndex++)
            Rules[ruleIndex].Check();
    }

    /// <summary>
    /// Adds a player to the game.
    /// </summary>
    /// <param name="player">The player to add to the game.</param>
    public abstract void AddPlayer(Player player);

    /// <summary>
    /// Gets the player who is currently taking his turn.
    /// </summary>
    /// <returns>The currently active player.</returns>
    public abstract Player GetCurrentPlayer();

    /// <summary>
    /// Deals cards to the participating players.
    /// </summary>
    public abstract void Deal();

    /// <summary>
    /// Initializes the game and lets the players start playing.
    /// </summary>
    public abstract void StartPlaying();
}