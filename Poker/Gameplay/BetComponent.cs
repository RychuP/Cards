using Framework.Assets;
using Framework.UI;
using Microsoft.Xna.Framework.Graphics;
using Poker.UI;
using Poker.UI.Screens;
using System.Collections.Generic;

namespace Poker.Gameplay;

class BetComponent : DrawableGameComponent
{
    readonly GameManager _gameManager;
    readonly Vector2[] _chipPositions = new Vector2[ChipAssets.ValueChips.Count];
    readonly List<AnimatedGameComponent> _currentChipComponents = new();

    public BetComponent(GameManager gm) : base(gm.Game)
    {
        _gameManager = gm;
    }

    public override void Initialize()
    {
        base.Initialize();

        // calculate positions for the chip that allow placing bets
        Rectangle chipSize = ChipAssets.ValueChips[ChipAssets.Values[0]].Bounds;
        int chipsCount = ChipAssets.Values.Length;
        int chipsWidthCombined = chipSize.Width * chipsCount;
        int chipsPaddingCombined = (chipsCount - 1) * Constants.ChipPadding; 
        int initialX = (Constants.GameWidth - chipsWidthCombined - chipsPaddingCombined) / 2;
        Vector2 chipPosition = new(initialX, 480);

        for (int i = 0; i < chipsCount; i++)
        {
            _chipPositions[i] = chipPosition;
            int offsetY = i switch
            {
                0 => 12,
                2 => -12,
                _ => 0
            };
            chipPosition += new Vector2(Constants.ChipPadding + chipSize.Width, offsetY);
        }
    }

    public override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);

        var sb = Game.Services.GetService<SpriteBatch>();
        sb.Begin();

        // draws betting chips
        for (int i = 0; i < _chipPositions.Length; i++)
        {
            int chipValue = ChipAssets.Values[i];
            Texture2D chipTexture = ChipAssets.ValueChips[chipValue];
            sb.Draw(chipTexture, _chipPositions[i], Color.White);
        }

        // check if gameplay screen is showing
        if (Game.Services.GetService<ScreenManager>().ActiveScreen is GameplayScreen)
        {
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
        }

        sb.End();
    }
}