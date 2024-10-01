#region File Description
//-----------------------------------------------------------------------------
// BlackjackGame.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameStateManagement;
using CardsFramework;
using System.Linq;

namespace Blackjack;

/// <summary>
/// The various possible game states.
/// </summary>
public enum BlackjackGameState
{
    Shuffling,
    Betting,
    Playing,
    Dealing,
    RoundEnd,
    GameOver,
}

class BlackjackCardGame : CardGame
{
    #region Fields and Properties
    readonly Dictionary<Player, string> _playerHandValueTexts = new();
    readonly Dictionary<Player, string> _playerSecondHandValueTexts = new();
    readonly Hand _usedCards = new();
    readonly BlackjackPlayer _dealerPlayer;
    readonly bool[] _turnFinishedByPlayer;
    readonly TimeSpan _dealDuration = TimeSpan.FromMilliseconds(500);

    readonly AnimatedHandGameComponent[] _animatedHands;
    // An additional list for managing hands created when performing a split.
    readonly AnimatedHandGameComponent[] _animatedSecondHands;

    BetGameComponent _betGameComponent;
    AnimatedHandGameComponent _dealerHandComponent;
    readonly Dictionary<string, Button> _buttons = new();
    Button _newGameButton;
    bool _showInsurance;

    // An offset used for drawing the second hand which appears after a split in
    // the correct location.
    Vector2 _secondHandOffset = new(100 * BlackjackGame.WidthScale, 25 * BlackjackGame.HeightScale);
    static readonly Vector2 RingOffset = new(0, 110);

    Vector2 _frameSize = new(180, 180);

    public BlackjackGameState State { get; set; }
    readonly ScreenManager _screenManager;

    const int MaxPlayers = 3;
    const int MinPlayers = 1;
    #endregion

    #region Initializations
    /// <summary>
    /// Creates a new instance of the <see cref="BlackjackCardGame"/> class.
    /// </summary>
    /// <param name="tableBounds">The table bounds. These serves as the bounds for 
    /// the game's main area.</param>
    /// <param name="dealerPosition">Position for the dealer's deck.</param>
    /// <param name="placeOrder">A method that translate a player index into the
    /// position of his deck on the game table.</param>
    /// <param name="screenManager">The games <see cref="ScreenManager"/>.</param>
    /// <param name="theme">The game's deck theme name.</param>
    public BlackjackCardGame(Rectangle tableBounds, Vector2 dealerPosition,
        Func<int, Vector2> placeOrder, ScreenManager screenManager, string theme)
        : base(6, 0, CardSuits.AllSuits, CardValues.NonJokers, MinPlayers, MaxPlayers, 
        new BlackJackTable(RingOffset, tableBounds, dealerPosition, MaxPlayers, 
            placeOrder, theme, screenManager.Game),
        theme, screenManager.Game)
    {
        _dealerPlayer = new BlackjackPlayer("Dealer", this);
        _turnFinishedByPlayer = new bool[MaximumPlayers];
        _screenManager = screenManager;
        _animatedHands ??= new AnimatedHandGameComponent[MaxPlayers];
        _animatedSecondHands ??= new AnimatedHandGameComponent[MaxPlayers];
    }

    /// <summary>
    /// Performs necessary initializations.
    /// </summary>
    public void Initialize()
    {
        LoadContent();

        // Initialize a new bet component
        _betGameComponent = new BetGameComponent(Players, this);
        Game.Components.Add(_betGameComponent);

        // Initialize the game buttons
        string[] buttonsText = { "Hit", "Stand", "Double", "Split", "Insurance" };
        for (int buttonIndex = 0; buttonIndex < buttonsText.Length; buttonIndex++)
        {
            Button button = new("ButtonRegular", "ButtonPressed", this)
            {
                Text = buttonsText[buttonIndex],
                Bounds = new Rectangle(_screenManager.SafeArea.Left + 10 + buttonIndex * 110,
                    _screenManager.SafeArea.Bottom - 60, 100, 50),
                Visible = false,
                Enabled = false
            };
            _buttons.Add(buttonsText[buttonIndex], button);
            Game.Components.Add(button);
        }

        _newGameButton = new Button("ButtonRegular", "ButtonPressed", this)
        {
            Text = "New Hand",
            Visible = false,
            Enabled = false,
            Bounds = new Rectangle(_screenManager.SafeArea.Left + 10,
                _screenManager.SafeArea.Bottom - 60, 200, 50),
        };

        // Alter the insurance button's bounds as it is considerably larger than
        // the other buttons
        Rectangle insuranceBounds = _buttons["Insurance"].Bounds;
        insuranceBounds.Width = 200;
        _buttons["Insurance"].Bounds = insuranceBounds;

        _newGameButton.Click += NewGameButton_OnClick;
        Game.Components.Add(_newGameButton);

        // Register to click event
        _buttons["Hit"].Click += HitButton_OnClick;
        _buttons["Stand"].Click += StandButton_OnClick;
        _buttons["Double"].Click += DoubleButton_OnClick;
        _buttons["Split"].Click += SplitButton_OnClick;
        _buttons["Insurance"].Click += InsuranceButton_OnClick;
    }
    #endregion

    #region Update and Render
    /// <summary>
    /// Perform the game's update logic.
    /// </summary>
    /// <param name="gameTime">Time elapsed since the last call to 
    /// this method.</param>
    public void Update()
    {
        switch (State)
        {
            case BlackjackGameState.Shuffling:
                ShowShuffleAnimation();
                break;
            
            case BlackjackGameState.Betting:
                EnableButtons(false);
                break;
            
            case BlackjackGameState.Dealing:
                // Deal 2 cards and start playing
                State = BlackjackGameState.Playing;
                Deal();
                StartPlaying();
                break;

            case BlackjackGameState.Playing:
                // Calculate players' current hand values
                foreach (BlackjackPlayer player in Players.Cast<BlackjackPlayer>())
                    player.CalculateValues();

                _dealerPlayer.CalculateValues();

                // Make sure no animations are running
                if (!CheckForRunningAnimations<AnimatedCardsGameComponent>())
                {
                    BlackjackPlayer player = (BlackjackPlayer)GetCurrentPlayer();
                    // If the current player is an AI player, make it play
                    if (player is BlackjackAIPlayer aiPlayer)
                        aiPlayer.AIPlay();

                    CheckRules();

                    // If all players have finished playing, the current round ends
                    if (State == BlackjackGameState.Playing && GetCurrentPlayer() == null)
                        EndRound();

                    // Update button availability according to player options
                    SetButtonAvailability();
                }
                else
                    EnableButtons(false);
                break;

            case BlackjackGameState.RoundEnd:
                if (_dealerHandComponent.EstimatedTimeForAnimationsCompletion() == TimeSpan.Zero)
                {
                    _betGameComponent.CalculateBalance(_dealerPlayer);
                    // Check if there is enough money to play
                    // then show new game option or tell the player he has lost
                    if (((BlackjackPlayer)Players[0]).Balance < 5)
                    {
                        EndGame();
                    }
                    else
                    {
                        _newGameButton.Enabled = true;
                        _newGameButton.Visible = true;
                    }
                }
                break;

            case BlackjackGameState.GameOver:
            default: break;
        }
    }

    /// <summary>
    /// Shows the card shuffling animation.
    /// </summary>
    private void ShowShuffleAnimation()
    {
        // Add shuffling animation
        AnimatedGameComponent animationComponent = new(Game)
        {
            Position = GameTable.DealerPosition,
            Visible = false
        };
        Game.Components.Add(animationComponent);

        animationComponent.AddAnimation(
            new FramesetGameComponentAnimation(CardAssets["shuffle_" + Theme], 32, 11, _frameSize)
        {
            Duration = TimeSpan.FromSeconds(1.5f),
            PerformBeforeStart = ShowComponent,
            PerformBeforeStartArgs = animationComponent,
            PerformWhenDone = PlayShuffleAndRemoveComponent,
            PerformWhenDoneArgs = animationComponent
        });
        State = BlackjackGameState.Betting;
    }

    /// <summary>
    /// Helper method to show component
    /// </summary>
    /// <param name="obj"></param>
    void ShowComponent(object obj)
    {
        ((AnimatedGameComponent)obj).Visible = true;
    }

    /// <summary>
    /// Helper method to play shuffle sound and remove component
    /// </summary>
    /// <param name="obj"></param>
    void PlayShuffleAndRemoveComponent(object obj)
    {
        AudioManager.PlaySound("Shuffle");
        Game.Components.Remove((AnimatedGameComponent)obj);
    }

    /// <summary>
    /// Renders the visual elements for which the game itself is responsible.
    /// </summary>
    /// <param name="gameTime">Time passed since the last call to 
    /// this method.</param>
    public void Draw(GameTime gameTime)
    {
        SpriteBatch.Begin();

        switch (State)
        {
            case BlackjackGameState.Playing:
                ShowPlayerValues(); 
                break;

            case BlackjackGameState.RoundEnd:
                if (_dealerHandComponent.EstimatedTimeForAnimationsCompletion() == TimeSpan.Zero)
                    ShowDealerValue();
                ShowPlayerValues();
                break;

            case BlackjackGameState.GameOver:
            default: break;
        }

        SpriteBatch.End();
    }

    /// <summary>
    /// Draws the dealer's hand value on the screen.
    /// </summary>
    private void ShowDealerValue()
    {
        // Calculate the value to display
        string dealerValue = _dealerPlayer.FirstValue.ToString();
        if (_dealerPlayer.FirstValueConsiderAce)
        {
            dealerValue = _dealerPlayer.FirstValue + 10 == 21 ? "21" :
                dealerValue + @"\" + (_dealerPlayer.FirstValue + 10).ToString();
        }

        // Draw the value
        Vector2 measure = Font.MeasureString(dealerValue);
        Vector2 position = GameTable.DealerPosition - new Vector2(measure.X + 20, 0);

        SpriteBatch.Draw(_screenManager.BlankTexture,
            new Rectangle((int)position.X - 4, (int)position.Y,
            (int)measure.X + 8, (int)measure.Y), Color.Black);

        SpriteBatch.DrawString(Font, dealerValue,
            position, Color.White);
    }

    /// <summary>
    /// Draws the players' hand value on the screen.
    /// </summary>
    private void ShowPlayerValues()
    {
        Color color = Color.Black;
        Player currentPlayer = GetCurrentPlayer();

        for (int playerIndex = 0; playerIndex < Players.Count; playerIndex++)
        {
            BlackjackPlayer player = (BlackjackPlayer)Players[playerIndex];

            // The current player's hand value will be red to serve as a visual
            // prompt for who the active player is
            color = player == currentPlayer ? Color.Red : Color.White;

            // Calculate the values to draw
            string playerHandValueText;
            string playerSecondHandValueText = null;
            if (!_animatedHands[playerIndex].IsAnimating)
            {
                if (player.FirstValue > 0)
                {
                    playerHandValueText = player.FirstValue.ToString();
                    // Take the fact that an ace is wither 1 or 11 into 
                    // consideration when calculating the value to display
                    // Since the ace already counts as 1, we add 10 to get
                    // the alternate value
                    if (player.FirstValueConsiderAce)
                    {
                        if (player.FirstValue + 10 == 21)
                        {
                            playerHandValueText = "21";
                        }
                        else
                        {
                            playerHandValueText += @"\" +
                                (player.FirstValue + 10).ToString();
                        }
                    }
                    _playerHandValueTexts[player] = playerHandValueText;
                }
                else
                {
                    playerHandValueText = null;
                }

                if (player.IsSplit)
                {
                    // If the player has performed a split, he has an additional
                    // hand with its own value
                    if (player.SecondValue > 0)
                    {
                        playerSecondHandValueText = player.SecondValue.ToString();
                        if (player.SecondValueConsiderAce)
                        {
                            if (player.SecondValue + 10 == 21)
                            {
                                playerSecondHandValueText = "21";
                            }
                            else
                            {
                                playerSecondHandValueText +=
                                    @"\" + (player.SecondValue + 10).ToString();
                            }
                        }
                        _playerSecondHandValueTexts[player] =
                            playerSecondHandValueText;
                    }
                    else
                    {
                        playerSecondHandValueText = null;
                    }
                }
            }
            else
            {
                _playerHandValueTexts.TryGetValue(player, out playerHandValueText);
                _playerSecondHandValueTexts.TryGetValue(
                    player, out playerSecondHandValueText);
            }

            if (player.IsSplit)
            {
                // If the player has performed a split, mark the active hand alone
                // with a red value
                color = player.CurrentHandType == HandTypes.First &&
                    player == currentPlayer ? Color.Red : Color.White;

                if (playerHandValueText != null)
                {
                    DrawValue(_animatedHands[playerIndex], playerIndex, playerHandValueText, color);
                }

                color = player.CurrentHandType == HandTypes.Second &&
                    player == currentPlayer ? Color.Red : Color.White;

                if (playerSecondHandValueText != null)
                {
                    DrawValue(_animatedSecondHands[playerIndex], playerIndex, playerSecondHandValueText,
                        color);
                }
            }
            else
            {
                // If there is a value to draw, draw it
                if (playerHandValueText != null)
                {
                    DrawValue(_animatedHands[playerIndex], playerIndex, playerHandValueText, color);
                }
            }
        }
    }

    /// <summary>
    /// Draws the value of a player's hand above his top card.
    /// The value will be drawn over a black background.
    /// </summary>
    /// <param name="animatedHand">The player's hand.</param>
    /// <param name="place">A number representing the player's position on the game table.</param>
    /// <param name="value">The value to draw.</param>
    /// <param name="valueColor">The color in which to draw the value.</param>
    private void DrawValue(AnimatedHandGameComponent animatedHand, int place, string value, Color valueColor)
    {
        Hand hand = animatedHand.Hand;

        Vector2 position = GameTable.PlaceOrder(place) + 
            animatedHand.GetCardRelativePosition(hand.Count - 1);
        Vector2 measure = Font.MeasureString(value);

        position.X += (CardAssets["CardBack_" + Theme].Bounds.Width - measure.X) / 2;
        position.Y -= measure.Y + 5;

        SpriteBatch.Draw(_screenManager.BlankTexture,
            new Rectangle((int)position.X - 4, (int)position.Y,
            (int)measure.X + 8, (int)measure.Y), Color.Black);
        SpriteBatch.DrawString(Font, value, position, valueColor);

    }
    #endregion

    #region Override Methods
    /// <summary>
    /// Adds a player to the game.
    /// </summary>
    /// <param name="player">The player to add.</param>
    public override void AddPlayer(Player player)
    {
        if (player is BlackjackPlayer && Players.Count < MaximumPlayers)
            Players.Add(player);
    }

    /// <summary>
    /// Gets the active player.
    /// </summary>
    /// <returns>The first payer who has placed a bet and has not 
    /// finish playing.</returns>
    public override Player GetCurrentPlayer()
    {
        for (int playerIndex = 0; playerIndex < Players.Count; playerIndex++)
            if (((BlackjackPlayer)Players[playerIndex]).MadeBet && _turnFinishedByPlayer[playerIndex] == false)
                return Players[playerIndex];
        return null;
    }

    /// <summary>
    /// Calculate the value of a blackjack card.
    /// </summary>
    /// <param name="card">The card to calculate the value for.</param>
    /// <returns>The card's value. All card values are equal to their face number,
    /// except for jack/queen/king which value at 10.</returns>
    /// <remarks>An ace's value will be 1. Game logic will treat it as 11 where
    /// appropriate.</remarks>
    public override int GetCardValue(TraditionalCard card) =>
        Math.Min(base.GetCardValue(card), 10);

    /// <summary>
    /// Deals 2 cards to each player including the dealer and adds the appropriate 
    /// animations.
    /// </summary>
    public override void Deal()
    {
        if (State == BlackjackGameState.Playing)
        {
            TraditionalCard card;
            for (int dealIndex = 0; dealIndex < 2; dealIndex++)
            {
                for (int playerIndex = 0; playerIndex < Players.Count; playerIndex++)
                {
                    if (((BlackjackPlayer)Players[playerIndex]).MadeBet)
                    {
                        // Deal a card to one of the players
                        card = Dealer.DealCardToHand(Players[playerIndex].Hand);

                        AddDealAnimation(card, _animatedHands[playerIndex], true, _dealDuration,
                            DateTime.Now + TimeSpan.FromSeconds(
                            _dealDuration.TotalSeconds * (dealIndex * Players.Count + playerIndex)));
                    }
                }
                // Deal a card to the dealer
                card = Dealer.DealCardToHand(_dealerPlayer.Hand);
                AddDealAnimation(card, _dealerHandComponent, dealIndex == 0, _dealDuration, DateTime.Now);
            }
        }
    }

    /// <summary>
    /// Performs necessary initializations needed after dealing the cards in order
    /// to start playing.
    /// </summary>
    public override void StartPlaying()
    {
        // Check that there are enough players to start playing
        if ((MinimumPlayers <= Players.Count && Players.Count <= MaximumPlayers))
        {
            // Set up and register to gameplay events
            GameRule gameRule = new BustRule(Players);
            Rules.Add(gameRule);
            gameRule.RuleMatch += BustRule_OnMatch;

            gameRule = new BlackJackRule(Players);
            Rules.Add(gameRule);
            gameRule.RuleMatch += BlackJackRule_OnMatch;

            gameRule = new InsuranceRule(_dealerPlayer.Hand);
            Rules.Add(gameRule);
            gameRule.RuleMatch += InsuranceRule_OnMatch;

            // Display the hands participating in the game
            for (int playerIndex = 0; playerIndex < Players.Count; playerIndex++)
            {
                if (((BlackjackPlayer)Players[playerIndex]).MadeBet)
                    _animatedHands[playerIndex].Visible = false;
                else
                    _animatedHands[playerIndex].Visible = true;
            }
        }
    }
    #endregion

    #region Helper Methods
    /// <summary>
    /// Display an animation when a card is dealt.
    /// </summary>
    /// <param name="card">The card being dealt.</param>
    /// <param name="animatedHand">The animated hand into which the card is dealt.</param>
    /// <param name="flipCard">Should the card be flipped after dealing it.</param>
    /// <param name="duration">The animations desired duration.</param>
    /// <param name="startTime">The time at which the animation should start.</param>
    public void AddDealAnimation(TraditionalCard card, AnimatedHandGameComponent
        animatedHand, bool flipCard, TimeSpan duration, DateTime startTime)
    {
        // Get the card location and card component
        int cardLocationInHand = animatedHand.GetCardLocationInHand(card);
        AnimatedCardsGameComponent cardComponent = animatedHand.GetCardGameComponent(cardLocationInHand);

        // Add the transition animation
        cardComponent.AddAnimation(
            new TransitionGameComponentAnimation(GameTable.DealerPosition,
            animatedHand.Position +
            animatedHand.GetCardRelativePosition(cardLocationInHand))
        {
            StartTime = startTime,
            PerformBeforeStart = ShowComponent,
            PerformBeforeStartArgs = cardComponent,
            PerformWhenDone = PlayDealSound
        });

        if (flipCard)
        {
            // Add the flip animation
            cardComponent.AddAnimation(new FlipGameComponentAnimation
            {
                IsFromFaceDownToFaceUp = true,
                Duration = duration,
                StartTime = startTime + duration,
                PerformWhenDone = PlayFlipSound
            });
        }
    }

    /// <summary>
    /// Helper method to play deal sound
    /// </summary>
    /// <param name="obj"></param>
    void PlayDealSound(object obj) =>
        AudioManager.PlaySound("Deal");

    /// <summary>
    /// Helper method to play flip sound
    /// </summary>
    /// <param name="obj"></param>
    void PlayFlipSound(object obj) =>
        AudioManager.PlaySound("Flip");

    /// <summary>
    /// Adds an animation which displays an asset over a player's hand. The asset
    /// will appear above the hand and appear to "fall" on top of it.
    /// </summary>
    /// <param name="player">The player over the hand of which to place the
    /// animation.</param>
    /// <param name="assetName">Name of the asset to display above the hand.</param>
    /// <param name="animationHand">Which hand to put cue over.</param>
    /// <param name="waitForHand">Start the cue animation when the animation
    /// of this hand over null of the animation of the currentHand</param>
    void CueOverPlayerHand(BlackjackPlayer player, string assetName,
        HandTypes animationHand, AnimatedHandGameComponent waitForHand)
    {
        // Get the position of the relevant hand
        int playerIndex = Players.IndexOf(player);
        AnimatedHandGameComponent currentAnimatedHand;
        Vector2 currentPosition;
        if (playerIndex >= 0)
        {
            switch (animationHand)
            {
                case HandTypes.First:
                    currentAnimatedHand = _animatedHands[playerIndex];
                    currentPosition = currentAnimatedHand.Position;
                    break;

                case HandTypes.Second:
                    currentAnimatedHand = _animatedSecondHands[playerIndex];
                    currentPosition = currentAnimatedHand.Position + _secondHandOffset;
                    break;

                default:
                    throw new Exception("Player has an unsupported hand type.");
            }
        }
        else
        {
            currentAnimatedHand = _dealerHandComponent;
            currentPosition = currentAnimatedHand.Position;
        }

        // Add the animation component 
        AnimatedGameComponent animationComponent = new(this, CardAssets[assetName])
        {
            Position = currentPosition,
            Visible = false
        };
        Game.Components.Add(animationComponent);

        // Calculate when to start the animation. The animation will only begin
        // after all hand cards finish animating
        TimeSpan estimatedTimeToCompleteAnimations;
        if (waitForHand != null)
        {
            estimatedTimeToCompleteAnimations = waitForHand.EstimatedTimeForAnimationsCompletion();
        }
        else
        {
            estimatedTimeToCompleteAnimations = currentAnimatedHand.EstimatedTimeForAnimationsCompletion();
        }

        // Add a scale effect animation
        animationComponent.AddAnimation(new ScaleGameComponentAnimation(2.0f, 1.0f)
        {
            StartTime = DateTime.Now + estimatedTimeToCompleteAnimations,
            Duration = TimeSpan.FromSeconds(1f),
            PerformBeforeStart = ShowComponent,
            PerformBeforeStartArgs = animationComponent
        });
    }

    /// <summary>
    /// Ends the current round.
    /// </summary>
    private void EndRound()
    {
        RevealDealerFirstCard();
        DealerAI();
        ShowResults();
        State = BlackjackGameState.RoundEnd;
    }

    /// <summary>
    /// Causes the dealer's hand to be displayed.
    /// </summary>
    private void ShowDealerHand()
    {
        _dealerHandComponent =
            new BlackjackAnimatedDealerHandComponent(-1, _dealerPlayer.Hand, this);
        Game.Components.Add(_dealerHandComponent);
    }

    /// <summary>
    /// Reveal's the dealer's hidden card.
    /// </summary>
    private void RevealDealerFirstCard()
    {
        // Iterate over all dealer cards expect for the last
        AnimatedCardsGameComponent cardComponent = _dealerHandComponent.GetCardGameComponent(1);
        cardComponent?.AddAnimation(new FlipGameComponentAnimation()
        {
            Duration = TimeSpan.FromSeconds(0.5),
            StartTime = DateTime.Now
        });
    }

    /// <summary>
    /// Present visual indication as to how the players fared in the current round.
    /// </summary>
    private void ShowResults()
    {
        // Calculate the dealer's hand value
        int dealerValue = _dealerPlayer.FirstValue;

        if (_dealerPlayer.FirstValueConsiderAce)
        {
            dealerValue += 10;
        }

        // Show each player's result
        for (int playerIndex = 0; playerIndex < Players.Count; playerIndex++)
        {
            ShowResultForPlayer((BlackjackPlayer)Players[playerIndex], dealerValue, HandTypes.First);
            if (((BlackjackPlayer)Players[playerIndex]).IsSplit)
            {
                ShowResultForPlayer((BlackjackPlayer)Players[playerIndex], dealerValue, HandTypes.Second);
            }
        }
    }

    /// <summary>
    /// Display's a player's status after the turn has ended.
    /// </summary>
    /// <param name="player">The player for which to display the status.</param>
    /// <param name="dealerValue">The dealer's hand value.</param>
    /// <param name="currentHandType">The player's hand to take into 
    /// account.</param>
    private void ShowResultForPlayer(BlackjackPlayer player, int dealerValue, HandTypes currentHandType)
    {
        // Calculate the player's hand value and check his state (blackjack/bust)
        bool blackjack, bust;
        int playerValue;
        switch (currentHandType)
        {
            case HandTypes.First:
                blackjack = player.BlackJack;
                bust = player.Bust;

                playerValue = player.FirstValue;

                if (player.FirstValueConsiderAce)
                    playerValue += 10;
                break;

            case HandTypes.Second:
                blackjack = player.SecondBlackJack;
                bust = player.SecondBust;

                playerValue = player.SecondValue;

                if (player.SecondValueConsiderAce)
                    playerValue += 10;
                break;

            default:
                throw new Exception("Player has an unsupported hand type.");
        }

        // The bust or blackjack state are animated independently of this method,
        // so only trigger different outcome indications
        if (player.MadeBet && (!blackjack || (_dealerPlayer.BlackJack && blackjack)) && !bust)
        {
            string assetName = GetResultAsset(player, dealerValue, playerValue);
            CueOverPlayerHand(player, assetName, currentHandType, _dealerHandComponent);
        }
    }

    /// <summary>
    /// Return the asset name according to the result.
    /// </summary>
    /// <param name="player">The player for which to return the asset name.</param>
    /// <param name="dealerValue">The dealer's hand value.</param>
    /// <param name="playerValue">The player's hand value.</param>
    /// <returns>The asset name</returns>
    private string GetResultAsset(BlackjackPlayer player, int dealerValue, int playerValue)
    {
        string assetName;
        if (_dealerPlayer.Bust)
        {
            assetName = "win";
        }
        else if (_dealerPlayer.BlackJack)
        {
            if (player.BlackJack)
            {
                assetName = "push";
            }
            else
            {
                assetName = "lose";
            }
        }
        else if (playerValue < dealerValue)
        {
            assetName = "lose";
        }
        else if (playerValue > dealerValue)
        {
            assetName = "win";
        }
        else
        {
            assetName = "push";
        }
        return assetName;
    }

    /// <summary>
    /// Have the dealer play. The dealer hits until reaching 17+ and then 
    /// stands.
    /// </summary>
    private void DealerAI()
    {
        // The dealer may have not need to draw additional cards after his first
        // two. Check if this is the case and if so end the dealer's play.
        _dealerPlayer.CalculateValues();
        int dealerValue = _dealerPlayer.FirstValue;

        if (_dealerPlayer.FirstValueConsiderAce)
        {
            dealerValue += 10;
        }

        if (dealerValue > 21)
        {
            _dealerPlayer.Bust = true;
            CueOverPlayerHand(_dealerPlayer, "bust", HandTypes.First, _dealerHandComponent);
        }
        else if (dealerValue == 21)
        {
            _dealerPlayer.BlackJack = true;
            CueOverPlayerHand(_dealerPlayer, "blackjack", HandTypes.First, _dealerHandComponent);
        }

        if (_dealerPlayer.BlackJack || _dealerPlayer.Bust)
        {
            return;
        }

        // Draw cards until 17 is reached, or the dealer gets a blackjack or busts
        int cardsDealed = 0;
        while (dealerValue <= 17)
        {
            TraditionalCard card = Dealer.DealCardToHand(_dealerPlayer.Hand);
            AddDealAnimation(card, _dealerHandComponent, true, _dealDuration,
                DateTime.Now.AddMilliseconds(1000 * (cardsDealed + 1)));
            cardsDealed++;
            _dealerPlayer.CalculateValues();
            dealerValue = _dealerPlayer.FirstValue;

            if (_dealerPlayer.FirstValueConsiderAce)
            {
                dealerValue += 10;
            }

            if (dealerValue > 21)
            {
                _dealerPlayer.Bust = true;
                CueOverPlayerHand(_dealerPlayer, "bust", HandTypes.First, _dealerHandComponent);
            }
        }
    }

    /// <summary>
    /// Displays the hands currently in play.
    /// </summary>
    private void DisplayPlayingHands()
    {
        for (int playerIndex = 0; playerIndex < Players.Count; playerIndex++)
        {
            AnimatedHandGameComponent animatedHandGameComponent =
                new BlackjackAnimatedPlayerHandComponent(playerIndex, Players[playerIndex].Hand, this);
            Game.Components.Add(animatedHandGameComponent);
            _animatedHands[playerIndex] = animatedHandGameComponent;
        }

        ShowDealerHand();
    }

    /// <summary>
    /// Starts a new game round.
    /// </summary>
    public void StartRound()
    {
        _playerHandValueTexts.Clear();
        AudioManager.PlaySound("Shuffle");
        Dealer.Shuffle();
        DisplayPlayingHands();
        State = BlackjackGameState.Shuffling;
    }

    /// <summary>
    /// Sets the button availability according to the options available to the 
    /// current player.
    /// </summary>
    private void SetButtonAvailability()
    {
        BlackjackPlayer player = (BlackjackPlayer)GetCurrentPlayer();
        // Hide all buttons if no player is in play or the player is an AI player
        if (player == null || player is BlackjackAIPlayer)
        {
            EnableButtons(false);
            ChangeButtonsVisiblility(false);
            return;
        }

        // Show all buttons
        EnableButtons(true);
        ChangeButtonsVisiblility(true);

        // Set insurance button availability
        _buttons["Insurance"].Visible = _showInsurance;
        _buttons["Insurance"].Enabled = _showInsurance;

        if (player.IsSplit == false)
        {
            // Remember that the bet amount was already reduced from the balance,
            // so we only need to check if the player has more money than the
            // current bet when trying to double/split

            // Set double button availability
            if (player.BetAmount > player.Balance || player.Hand.Count != 2)
            {
                _buttons["Double"].Visible = false;
                _buttons["Double"].Enabled = false;
            }

            if (player.Hand.Count != 2 ||
                player.Hand[0].Value != player.Hand[1].Value ||
                player.BetAmount > player.Balance)
            {
                _buttons["Split"].Visible = false;
                _buttons["Split"].Enabled = false;
            }
        }
        else
        {
            // We've performed a split. Get the initial bet amount to check whether
            // or not we can double the current bet.
            float initialBet = player.BetAmount /
                ((player.Double ? 2f : 1f) + (player.SecondDouble ? 2f : 1f));

            // Set double button availability.
            if (initialBet > player.Balance || player.CurrentHand.Count != 2)
            {
                _buttons["Double"].Visible = false;
                _buttons["Double"].Enabled = false;
            }

            // Once you've split, you can't split again
            _buttons["Split"].Visible = false;
            _buttons["Split"].Enabled = false;
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
    /// Ends the game.
    /// </summary>
    private void EndGame()
    {
        // Calculate the estimated time for all playing animations to end
        long estimatedTime = 0;
        for (int componentIndex = 0; componentIndex < Game.Components.Count; componentIndex++)
        {
            if (Game.Components[componentIndex] is AnimatedGameComponent animComp)
            {
                estimatedTime = Math.Max(estimatedTime, 
                    animComp.EstimatedTimeForAnimationsCompletion().Ticks);
            }
        }

        // Add a component for an empty stalling animation. This actually acts
        // as a timer.
        Texture2D texture = Game.Content.Load<Texture2D>(@"Images\youlose");
        AnimatedGameComponent animationComponent = new(this, texture)
        {
            Position = new Vector2(
                Game.GraphicsDevice.Viewport.Bounds.Center.X - texture.Width / 2,
                Game.GraphicsDevice.Viewport.Bounds.Center.Y - texture.Height / 2),
            Visible = false
        };
        Game.Components.Add(animationComponent);

        // Add a button to return to the main menu
        Rectangle bounds = Game.GraphicsDevice.Viewport.Bounds;
        Vector2 center = new(bounds.Center.X, bounds.Center.Y);
        Button backButton = new("ButtonRegular", "ButtonPressed", this)
        {
            Bounds = new Rectangle((int)center.X - 100, (int)center.Y + 80, 200, 50),
            Text = "Main Menu",
            Visible = false,
            Enabled = true,
        };

        backButton.Click += BackButton_OnClick;

        // Add stalling animation
        animationComponent.AddAnimation(new AnimatedGameComponentAnimation()
        {
            Duration = TimeSpan.FromTicks(estimatedTime) + TimeSpan.FromSeconds(1),
            PerformWhenDone = ResetGame,
            PerformWhenDoneArgs = new object[] { animationComponent, backButton }
        });
        Game.Components.Add(backButton);
    }

    /// <summary>
    /// Helper method to reset the game
    /// </summary>
    /// <param name="obj"></param>
    void ResetGame(object obj)
    {
        object[] arr = (object[])obj;
        State = BlackjackGameState.GameOver;
        ((AnimatedGameComponent)arr[0]).Visible = true;
        ((Button)arr[1]).Visible = true;

        // Remove all unnecessary game components
        for (int componentIndex = 0; componentIndex < Game.Components.Count; )
        {
            if ((Game.Components[componentIndex] != ((AnimatedGameComponent)arr[0]) &&
                Game.Components[componentIndex] != ((Button)arr[1])) &&
                (Game.Components[componentIndex] is BetGameComponent ||
                Game.Components[componentIndex] is AnimatedGameComponent ||
                Game.Components[componentIndex] is Button))
            {
                Game.Components.RemoveAt(componentIndex);
            }
            else
                componentIndex++;
        }
    }

    /// <summary>
    /// Finishes the current turn.
    /// </summary>
    private void FinishTurn()
    {
        // Remove all unnecessary components
        for (int componentIndex = 0; componentIndex < Game.Components.Count; componentIndex++)
        {
            if (!(Game.Components[componentIndex] is GameTable ||
                Game.Components[componentIndex] is BlackjackCardGame ||
                Game.Components[componentIndex] is BetGameComponent ||
                Game.Components[componentIndex] is Button ||
                Game.Components[componentIndex] is ScreenManager ||
                Game.Components[componentIndex] is InputHelper))
            {
                if (Game.Components[componentIndex] is AnimatedCardsGameComponent animatedCard)
                {
                    animatedCard.AddAnimation(
                        new TransitionGameComponentAnimation(animatedCard.Position,
                        new Vector2(animatedCard.Position.X, Game.GraphicsDevice.Viewport.Height))
                        {
                            Duration = TimeSpan.FromSeconds(0.40),
                            PerformWhenDone = RemoveComponent,
                            PerformWhenDoneArgs = animatedCard
                        });
                }
                else
                {
                    Game.Components.RemoveAt(componentIndex);
                    componentIndex--;
                }
            }
        }

        // Reset player values
        for (int playerIndex = 0; playerIndex < Players.Count; playerIndex++)
        {
            if (Players[playerIndex] is BlackjackPlayer player)
            {
                player.ResetValues();
                player.Hand.DealCardsToHand(_usedCards, player.Hand.Count);
                _turnFinishedByPlayer[playerIndex] = false;
                _animatedHands[playerIndex] = null;
                _animatedSecondHands[playerIndex] = null;
            }
        }

        // Reset the bet component
        _betGameComponent.Reset();
        _betGameComponent.Enabled = true;

        // Reset dealer
        _dealerPlayer.Hand.DealCardsToHand(_usedCards, _dealerPlayer.Hand.Count);
        _dealerPlayer.ResetValues();

        // Reset rules
        Rules.Clear();
    }

    /// <summary>
    /// Helper method to remove component
    /// </summary>
    /// <param name="obj"></param>
    void RemoveComponent(object obj)
    {
        Game.Components.Remove((AnimatedGameComponent)obj);
    }

    /// <summary>
    /// Performs the "Stand" move for the current player.
    /// </summary>
    public void Stand()
    {
        if (GetCurrentPlayer() is not BlackjackPlayer player) return;

        // If the player only has one hand, his turn ends. Otherwise, he now plays
        // using his next hand
        if (player.IsSplit == false)
        {
            _turnFinishedByPlayer[Players.IndexOf(player)] = true;
        }
        else
        {
            switch (player.CurrentHandType)
            {
                case HandTypes.First:
                    if (player.SecondBlackJack)
                        _turnFinishedByPlayer[Players.IndexOf(player)] = true;
                    else
                        player.CurrentHandType = HandTypes.Second;
                    break;

                case HandTypes.Second:
                    _turnFinishedByPlayer[Players.IndexOf(player)] = true;
                    break;

                default:
                    throw new Exception("Player has an unsupported hand type.");
            }
        }
    }

    /// <summary>
    /// Performs the "Split" move for the current player.
    /// This includes adding the animations which shows the first hand splitting
    /// into two.
    /// </summary>
    public void Split()
    {
        if (GetCurrentPlayer() is not BlackjackPlayer player) 
            throw new Exception("Calling split with no current player.");

        int playerIndex = Players.IndexOf(player);
        if (_animatedHands[playerIndex] is not AnimatedHandGameComponent animatedHand) 
            throw new Exception("Animated hand not initialized.");

        if (animatedHand.GetCardGameComponent(1) is not AnimatedCardsGameComponent animatedCard1 ||
            animatedHand.GetCardGameComponent(0) is not AnimatedCardsGameComponent animatedCard0)
            throw new InvalidOperationException("Missing card game components.");

        player.InitializeSecondHand();

        Vector2 sourcePosition = animatedCard1.Position;
        Vector2 targetPosition = animatedCard0.Position + _secondHandOffset;

        // Create an animation moving the top card to the second hand location
        AnimatedGameComponentAnimation animation = new TransitionGameComponentAnimation(sourcePosition,
                targetPosition)
        {
            StartTime = DateTime.Now,
            Duration = TimeSpan.FromSeconds(0.5f)
        };

        // Actually perform the split
        player.SplitHand();

        // Add additional chip stack for the second hand
        _betGameComponent.AddChips(playerIndex, player.BetAmount, false, true);

        // Initialize visual representation of the second hand
        var animatedSecondHand = new BlackjackAnimatedPlayerHandComponent(playerIndex, 
            _secondHandOffset, player.SecondHand, this);
        _animatedSecondHands[playerIndex] = animatedSecondHand;
        Game.Components.Add(animatedSecondHand);

        AnimatedCardsGameComponent animatedGameComponet = animatedSecondHand.GetCardGameComponent(0);
        animatedGameComponet.IsFaceDown = false;
        animatedGameComponet.AddAnimation(animation);

        // Deal an additional cards to each of the new hands
        TraditionalCard card = Dealer.DealCardToHand(player.Hand);
        AddDealAnimation(card, _animatedHands[playerIndex], true, _dealDuration,
            DateTime.Now + animation.EstimatedTimeForAnimationCompletion);
        card = Dealer.DealCardToHand(player.SecondHand);
        AddDealAnimation(card, _animatedSecondHands[playerIndex], true, _dealDuration,
            DateTime.Now + animation.EstimatedTimeForAnimationCompletion +
            _dealDuration);
    }

    /// <summary>
    /// Performs the "Double" move for the current player.
    /// </summary>
    public void Double()
    {
        BlackjackPlayer player = (BlackjackPlayer)GetCurrentPlayer();

        int playerIndex = Players.IndexOf(player);

        switch (player.CurrentHandType)
        {
            case HandTypes.First:
                player.Double = true;
                float betAmount = player.BetAmount;

                if (player.IsSplit)
                {
                    betAmount /= 2f;
                }

                _betGameComponent.AddChips(playerIndex, betAmount, false, false);
                break;

            case HandTypes.Second:
                player.SecondDouble = true;
                if (player.Double == false)
                {
                    // The bet is evenly spread between both hands, add one half
                    _betGameComponent.AddChips(playerIndex, player.BetAmount / 2f, false, true);
                }
                else
                {
                    // The first hand's bet is double, add one third of the total
                    _betGameComponent.AddChips(playerIndex, player.BetAmount / 3f, false, true);
                }
                break;

            default:
                throw new Exception("Player has an unsupported hand type.");
        }

        Hit();
        Stand();
    }

    /// <summary>
    /// Performs the "Hit" move for the current player.
    /// </summary>
    public void Hit()
    {
        BlackjackPlayer player = (BlackjackPlayer)GetCurrentPlayer();
        if (player == null)
            return;

        int playerIndex = Players.IndexOf(player);

        // Draw a card to the appropriate hand
        switch (player.CurrentHandType)
        {
            case HandTypes.First:
                TraditionalCard card = Dealer.DealCardToHand(player.Hand);
                AddDealAnimation(card, _animatedHands[playerIndex], true,
                    _dealDuration, DateTime.Now);
                break;
            case HandTypes.Second:
                card = Dealer.DealCardToHand(player.SecondHand);
                AddDealAnimation(card, _animatedSecondHands[playerIndex], true,
                    _dealDuration, DateTime.Now);
                break;
            default:
                throw new Exception(
                    "Player has an unsupported hand type.");
        }
    }

    /// <summary>
    /// Changes the visiblility of most game buttons.
    /// </summary>
    /// <param name="visible">True to make the buttons visible, false to make
    /// them invisible.</param>
    void ChangeButtonsVisiblility(bool visible)
    {
        _buttons["Hit"].Visible = visible;
        _buttons["Stand"].Visible = visible;
        _buttons["Double"].Visible = visible;
        _buttons["Split"].Visible = visible;
        _buttons["Insurance"].Visible = visible;
    }

    /// <summary>
    /// Enables or disable most game buttons.
    /// </summary>
    /// <param name="enabled">True to enable the buttons , false to 
    /// disable them.</param>
    void EnableButtons(bool enabled)
    {
        _buttons["Hit"].Enabled = enabled;
        _buttons["Stand"].Enabled = enabled;
        _buttons["Double"].Enabled = enabled;
        _buttons["Split"].Enabled = enabled;
        _buttons["Insurance"].Enabled = enabled;
    }

    /// <summary>
    /// Add an indication that the player has passed on the current round.
    /// </summary>
    /// <param name="indexPlayer">The player's index.</param>
    public void ShowPlayerPass(int indexPlayer)
    {
        // Add animation component
        AnimatedGameComponent passComponent = new(this, CardAssets["pass"])
        {
            Position = GameTable.PlaceOrder(indexPlayer),
            Visible = false
        };
        Game.Components.Add(passComponent);

        // Hide insurance button only when the first payer passes
        Action<object> performWhenDone = null;
        if (indexPlayer == 0)
        {
            performWhenDone = HideInshurance;
        }
        // Add scale animation for the pass "card"
        passComponent.AddAnimation(new ScaleGameComponentAnimation(2.0f, 1.0f)
        {
            AnimationCycles = 1,
            PerformBeforeStart = ShowComponent,
            PerformBeforeStartArgs = passComponent,
            StartTime = DateTime.Now,
            Duration = TimeSpan.FromSeconds(1),
            PerformWhenDone = performWhenDone
        });
    }

    /// <summary>
    /// Helper method to hide insurance
    /// </summary>
    /// <param name="obj"></param>
    void HideInshurance(object obj)
    {
        _showInsurance = false;
    }
    #endregion

    #region Event Handlers
    /// <summary>
    /// Shows the insurance button if the first player can afford insurance.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing 
    /// the event data.</param>
    void InsuranceRule_OnMatch(object sender, EventArgs e)
    {
        BlackjackPlayer player = (BlackjackPlayer)Players[0];
        if (player.Balance >= player.BetAmount / 2)
            _showInsurance = true;
    }

    /// <summary>
    /// Shows the bust visual cue after the bust rule has been matched.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing 
    /// the event data.</param>
    void BustRule_OnMatch(object sender, EventArgs e)
    {
        if (e is not BlackjackGameEventArgs args) return;

        _showInsurance = false;
        BlackjackPlayer player = (BlackjackPlayer)args.Player;

        CueOverPlayerHand(player, "bust", args.Hand, null);

        switch (args.Hand)
        {
            case HandTypes.First:
                player.Bust = true;

                if (player.IsSplit && !player.SecondBlackJack)
                {
                    player.CurrentHandType = HandTypes.Second;
                }
                else
                {
                    _turnFinishedByPlayer[Players.IndexOf(player)] = true;
                }
                break;
            case HandTypes.Second:
                player.SecondBust = true;
                _turnFinishedByPlayer[Players.IndexOf(player)] = true;
                break;
            default:
                throw new Exception(
                    "Player has an unsupported hand type.");
        }
    }

    /// <summary>
    /// Shows the blackjack visual cue after the blackjack rule has been matched.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing 
    /// the event data.</param>
    void BlackJackRule_OnMatch(object sender, EventArgs e)
    {
        if (e is not BlackjackGameEventArgs args) return;

        _showInsurance = false;
        BlackjackPlayer player = (BlackjackPlayer)args.Player;

        CueOverPlayerHand(player, "blackjack", args.Hand, null);

        switch (args.Hand)
        {
            case HandTypes.First:
                player.BlackJack = true;

                if (player.IsSplit)
                {
                    player.CurrentHandType = HandTypes.Second;
                }
                else
                {
                    _turnFinishedByPlayer[Players.IndexOf(player)] = true;
                }
                break;
            case HandTypes.Second:
                player.SecondBlackJack = true;
                if (player.CurrentHandType == HandTypes.Second)
                {
                    _turnFinishedByPlayer[Players.IndexOf(player)] = true;
                }
                break;
            default:
                throw new Exception(
                    "Player has an unsupported hand type.");
        }
    }

    /// <summary>
    /// Handles the Click event of the insurance button.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The 
    /// <see cref="System.EventArgs"/> instance containing the event data.</param>
    void InsuranceButton_OnClick(object sender, EventArgs e)
    {
        BlackjackPlayer player = (BlackjackPlayer)GetCurrentPlayer();
        if (player == null)
            return;
        player.IsInsurance = true;
        player.Balance -= player.BetAmount / 2f;
        _betGameComponent.AddChips(Players.IndexOf(player), player.BetAmount / 2, true, false);
        _showInsurance = false;
    }

    /// <summary>
    /// Handles the Click event of the new game button.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The 
    /// <see cref="EventArgs"/> instance containing the event data.</param>
    void NewGameButton_OnClick(object sender, EventArgs e)
    {
        FinishTurn();
        StartRound();
        _newGameButton.Enabled = false;
        _newGameButton.Visible = false;
    }

    /// <summary>
    /// Handles the Click event of the hit button.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The 
    /// <see cref="EventArgs"/> instance containing the event data.</param>
    void HitButton_OnClick(object sender, EventArgs e)
    {
        Hit();
        _showInsurance = false;
    }

    /// <summary>
    /// Handles the Click event of the stand button.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The 
    /// <see cref="System.EventArgs"/> instance containing the event data.</param>
    void StandButton_OnClick(object sender, EventArgs e)
    {
        Stand();
        _showInsurance = false;
    }

    /// <summary>
    /// Handles the Click event of the double button.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The 
    /// <see cref="System.EventArgs"/> instance containing the event data.</param>
    void DoubleButton_OnClick(object sender, EventArgs e)
    {
        Double();
        _showInsurance = false;
    }

    /// <summary>
    /// Handles the Click event of the split button.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The 
    /// <see cref="System.EventArgs"/> instance containing the event data.</param>
    void SplitButton_OnClick(object sender, EventArgs e)
    {
        Split();
        _showInsurance = false;
    }

    /// <summary>
    /// Handles the Click event of the back button.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">>The 
    /// <see cref="System.EventArgs"/> instance containing the event data.</param>
    void BackButton_OnClick(object sender, EventArgs e)
    {
        // Remove all unnecessary components
        for (int componentIndex = 0; componentIndex < Game.Components.Count; componentIndex++)
        {
            if (!(Game.Components[componentIndex] is ScreenManager))
            {
                Game.Components.RemoveAt(componentIndex);
                componentIndex--;
            }
        }

        foreach (GameScreen screen in _screenManager.GetScreens())
            screen.ExitScreen();

        _screenManager.AddScreen(new BackgroundScreen(), null);
        _screenManager.AddScreen(new MainMenuScreen(), null);
    }
    #endregion
}