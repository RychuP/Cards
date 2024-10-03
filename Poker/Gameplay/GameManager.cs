using Framework.Assets;
using Framework.Engine;
using Framework.Misc;
using Framework.UI;
using Poker.Gameplay.Players;
using Poker.UI;
using Poker.UI.Screens;
using System;
using System.Collections.Generic;

namespace Poker.Gameplay;

class GameManager : CardGame
{
    readonly CardPile _cardPile;
    readonly CommunityCards _communityCards;
    readonly BetComponent _betComponent;

    public PokerGameState State { get; set; }

    public GameManager(Game game) : base(1, 0, CardSuits.AllSuits, CardValues.NonJokers, Fonts.Moire.Regular,
        Constants.MinPlayers, Constants.MaxPlayers, new PokerTable(game), Constants.DefaultTheme, game)
    {
        // create card pile
        _cardPile = new CardPile(this);
        game.Components.Add(_cardPile);

        // create players
        AddPlayer(new HumanPlayer(Gender.Male, this));
        for (int place = 1; place < MaximumPlayers; place++)
        {
            (string name, Gender gender) = GetRandomPerson();
            AddPlayer(new AIPlayer(name, gender, place, this));
        }

        // create community cards cards holder
        _communityCards = new CommunityCards(this);

        // create bet component
        _betComponent = new BetComponent(this);
        game.Components.Add(_betComponent);
    }

    public void Update()
    {
        if (((PokerGame)Game).ScreenManager.ActiveScreen is not GameplayScreen) return;

        switch (State)
        {
            case PokerGameState.Shuffling:
                if (!CheckForRunningAnimations<CardPile>())
                {
                    State = PokerGameState.Dealing;
                    Deal();
                }
                break;

            case PokerGameState.Dealing:
                if (!CheckForRunningAnimations<AnimatedCardGameComponent>())
                {
                    State = PokerGameState.FirstBet;
                    _cardPile.SlideUp();
                }
                break;

            case PokerGameState.FirstBet:
                if (!CheckForRunningAnimations<CardPile>())
                {
                    
                }
                break;
        }
    }

    public override void StartPlaying()
    {
        State = PokerGameState.Shuffling;

        Dealer.Shuffle();
        _cardPile.ShowAndShuffle();
    }

    public override void Deal()
    {
        DateTime startTime = DateTime.Now;

        // deal player cards
        for (int dealIndex = 0; dealIndex < 2; dealIndex++)
        {
            for (int playerIndex = 0; playerIndex < Players.Count; playerIndex++)
            {
                var player = this[playerIndex];
                TraditionalCard card = Dealer.DealCardToHand(player.Hand);
                bool flip = player is HumanPlayer;

                // calculate start time and add deal animation
                player.AddDealAnimation(card, flip, startTime);
                startTime += Constants.DealAnimationDuration;
            }
        }

        // deal community cards
        for (int i = 0; i < 3; i++)
        {
            TraditionalCard card = Dealer.DealCardToHand(_communityCards.Hand);
            _communityCards.AddDealAnimation(card, true, startTime);
            startTime += Constants.DealAnimationDuration;
        }
    }

    /// <summary>
    /// Checks for running animations.
    /// </summary>
    /// <typeparam name="T">The type of animation to look for.</typeparam>
    /// <returns>True if a running animation of the desired type is found and
    /// false otherwise.</returns>
    internal bool CheckForRunningAnimations<T>() where T : AnimatedGameComponent
    {
        for (int componentIndex = 0; componentIndex < Game.Components.Count; componentIndex++)
            if (Game.Components[componentIndex] is T animComponent && animComponent.IsAnimating)
                return true;
        return false;
    }

    /// <summary>
    /// Indexer that returns <see cref="PokerBettingPlayer"/>.
    /// </summary>
    /// <returns><see cref="PokerBettingPlayer"/> with the given index.</returns>
    public PokerBettingPlayer this[int index] =>
        Players[index] as PokerBettingPlayer;

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

    /// <summary>
    /// Selects a random, unique name of an alternating gender.
    /// </summary>
    /// <returns></returns>
    (string, Gender) GetRandomPerson()
    {
        string name = string.Empty;
        Gender gender = (Gender)((PokerGame)Game).Random.Next(2);

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
        } while (Players.Find(p => p.Name == name) is PokerCardsHolder);
        
        return (name, gender);
    }

    /// <summary>
    /// Adds a player to the game.
    /// </summary>
    /// <param name="player">The player to add.</param>
    public override void AddPlayer(Player player)
    {
        if (player is PokerCardsHolder && Players.Count < MaximumPlayers)
            Players.Add(player);
    }

    public override void CheckRules()
    {
        base.CheckRules();
    }

    public override Player GetCurrentPlayer()
    {
        throw new NotImplementedException();
    }

    void OnThemeChanged(string theme)
    {
        ThemeChanged?.Invoke(this, new ThemeChangedEventArgs(theme));
    }

    public event EventHandler<ThemeChangedEventArgs> ThemeChanged;
}