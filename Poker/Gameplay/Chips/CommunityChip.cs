using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Poker.UI;
using System;

namespace Poker.Gameplay.Chips;

class CommunityChip : ValueChip
{
    // chip area used for detecting mouse hovers and clicks
    readonly Rectangle _area;

    // destination for drawing chip outline
    public readonly Rectangle OutlineDestination;

    public bool IsHover { get; private set; } = false;
    public bool IsClick { get; private set; } = false;

    public CommunityChip(Game game, int value) : base(game, value)
    {
        Point pos = GetTablePosition().ToPoint();
        _area = new Rectangle(pos, Size);

        // calculate outline destination
        var texture = Art.ChipOutline.Bounds;
        int offsetX = (texture.Width - _area.Width) / 2 + 2;
        int offsetY = (texture.Height - _area.Height) / 2 + 2;
        Point outlinePos = pos - new Point(offsetX, offsetY);
        OutlineDestination = new Rectangle(outlinePos, texture.Size);
    }

    public void Update()
    {
        IsHover = false;
        IsClick = false;

        if (IsAnimating)
        {
            return;
        }

        var mouseState = Mouse.GetState();
        if (_area.Contains(mouseState.Position))
        {
            IsHover = true;

            // check for clicks
            if (mouseState.LeftButton == ButtonState.Released &&
                InputManager.PrevMouseState.LeftButton == ButtonState.Pressed)
                IsClick = true;
        }
    }
}