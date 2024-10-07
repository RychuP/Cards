using Framework.Assets;
using Framework.UI;
using Microsoft.Xna.Framework.Graphics;
using Poker.Gameplay.Chips;
using Poker.Gameplay.Players;
using Poker.UI;
using Poker.UI.Screens;
using System;
using System.Collections.Generic;

namespace Poker.Gameplay;

class BetComponent : DrawableGameComponent
{
    readonly GameManager _gameManager;

    /// <summary>
    /// Player who receives the small blind chip this round.
    /// </summary>
    PokerBettingPlayer _smallBlindPlayer;

    /// <summary>
    /// Community chips that are placed below community cards.
    /// </summary>
    readonly List<ValueChip> CommunityChips = new(ValueChip.Count);

    /// <summary>
    /// Blind chips that are distrubued among the players.
    /// </summary>
    readonly List<BlindChip> BlindChips = new(BlindChip.Count);

    public BetComponent(GameManager gm) : base(gm.Game)
    {
        _gameManager = gm;
        Enabled = false;
        Visible = false;

        // create community chips
        for (int i = 0; i < ValueChip.Count; i++)
        {
            var chip = new ValueChip(Game, Chip.HiddenPosition, ValueChip.Values[i]);
            CommunityChips.Add(chip);
        }

        // create blind chips
        BlindChips.Add(new SmallBlindChip(Game, Chip.HiddenPosition));
        BlindChips.Add(new BigBlindChip(Game, Chip.HiddenPosition));
    }
    
    public override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);

        var sb = Game.Services.GetService<SpriteBatch>();
        sb.Begin();

        // draw balances and bet amounts
        for (int i = 0; i < Constants.MaxPlayers; i++)
        {
            var player = _gameManager[i];
            string balanceText = $"${player.Balance}";
            var balancePos = player.GetCenteredTextPosition(balanceText, 1);
            sb.DrawString(_gameManager.Font, balanceText, balancePos, Color.CornflowerBlue);

            string betText = $"${player.BetAmount}";
            var betPos = player.GetCenteredTextPosition(betText, 2);
            sb.DrawString(_gameManager.Font, betText, betPos, Color.OrangeRed);
        }

        sb.End();
    }

    public void Reset()
    {
        Enabled = true;
        Visible = true;

        var rand = Game.Services.GetService<Random>();

        // allocate a random player for the small blind token
        _smallBlindPlayer = _gameManager[rand.Next(0, Constants.MaxPlayers)];

        // put the community chips back in the hidden position
        foreach (var chip in CommunityChips)
            chip.Position = Chip.HiddenPosition;
        foreach (var chip in BlindChips)
            chip.Position = Chip.HiddenPosition;

        // remove all chips from players
    }
    
    public void ShowCommunityChips()
    {
        foreach (var chip in CommunityChips)
            chip.Position = chip.GetTablePosition();

        foreach (var chip in BlindChips)
            chip.Position = chip.GetTablePosition();
    }
}