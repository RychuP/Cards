using Framework.UI;
using Microsoft.Xna.Framework.Graphics;
using Solitaire.Managers;
using Solitaire.Misc;

namespace Solitaire.UI.BaseScreens;

internal class AnimatedTitle : AnimatedGameComponent
{
    public event EventHandler<DirectionEventArgs> DestinationReached;

    public static readonly Color DarkAreaColor = new(44, 103, 45);//  (47, 107, 48);

    // where the dark area starts compared with the dimensions of the title texture
    static readonly int DarkAreaOffset = 50;

    readonly Vector2 _defaultPosition;
    readonly MenuScreen _menuScreen;

    public Vector2 BottomPosition { get; }
    public Vector2 TopPosition { get; }

    /// <summary>
    /// Darkened area below the title shown when the title is in its top position.
    /// </summary>
    /// <remarks>Meant to be the background of a window.</remarks>
    public Texture2D DarkArea { get; }
    public Vector2 DarkAreaPosition { get; }
    GameManager GameManager => _menuScreen.GameManager;

    public AnimatedTitle(MenuScreen menuScreen, Texture2D texture, Direction defaultPosition) 
        : base(menuScreen.GameManager, texture)
    {
        _menuScreen = menuScreen;
        Visible = false;

        // calculate default positions
        int y = MenuScreen.LogoMarginTop + Art.CardsLogo.Height - 70;
        BottomPosition = new Vector2(GameScreen.GetCenteredPosition(texture.Bounds).X, y);
        TopPosition = new Vector2(BottomPosition.X, 83);
        _defaultPosition = defaultPosition == Direction.Down ? BottomPosition : TopPosition;
        Position = _defaultPosition;

        // create dark area texture
        var height = BottomPosition.Y - TopPosition.Y + Texture.Height / 2;
        var offset = Position.X + DarkAreaOffset;
        var width = SolitaireGame.Width - (int)offset * 2;
        DarkArea = ScreenManager.CreateTexture(Game.GraphicsDevice, width, (int)height, DarkAreaColor);

        // calculate dark area position
        int x = (SolitaireGame.Width - width) / 2;
        DarkAreaPosition = new Vector2(x, TopPosition.Y + Texture.Height / 2);

        // register event handlers
        _menuScreen.VisibleChanged += (o, e) => Visible = _menuScreen.Visible;
    }

    public override void Initialize()
    {
        // register event handlers
        GameManager.ScreenManager.ScreenChanged += ScreenManager_OnScreenChanged;

        base.Initialize();
    }

    public override void Draw(GameTime gameTime)
    {
        var sb = Game.Services.GetService<SpriteBatch>();
        sb.Begin();

        // draw the dark area below title if it's position is not at the bottom
        if (Position != BottomPosition)
        {
            var height = (int)BottomPosition.Y - (int)Position.Y + Texture.Height / 2;
            var offset = Position.X + DarkAreaOffset;
            var width = SolitaireGame.Width - (int)offset * 2;
            //var texture = ScreenManager.CreateTexture(Game.GraphicsDevice, width, height, DarkAreaColor);
            int x = (SolitaireGame.Width - width) / 2;
            var y = (int)Position.Y + Texture.Height / 2;

            var dest = new Rectangle(x, y, width, height);
            var source = new Rectangle(0, 0, width, height);
            sb.Draw(DarkArea, dest, source, Color.White);
        }

        // Draw the current frame at the designated destination, or at the initial 
        // position if a destination has not been set
        if (Texture != null)
        {
            if (Destination.HasValue)
                sb.Draw(Texture, Destination.Value, Color.White);
            else
                sb.Draw(Texture, Position, Color.White);
        }

        sb.End();
    }

    void GoTo(Vector2 destination)
    {
        RemoveAnimations();
        var direction = destination == TopPosition ? Direction.Up : Direction.Down;
        AddAnimation(new TransitionGameComponentAnimation(Position, destination)
        {
            PerformWhenDone = (o) => OnDestinationReached(direction)
        });
    }

    void OnDestinationReached(Direction direction)
    {
        var args = new DirectionEventArgs(direction);
        DestinationReached?.Invoke(this, args);
    }

    void ScreenManager_OnScreenChanged(object o, ScreenChangedEventArgs e)
    {
        if (e.NewScreen == _menuScreen)
        {
            if (e.PrevScreen is MenuScreen prevMenuScreen)
            {
                var position = new Vector2(Position.X, prevMenuScreen.Title.Position.Y);
                if (Position != position)
                {
                    Position = position;
                    GoTo(_defaultPosition);
                }
            }
        }
        else
        {
            Position = _defaultPosition;
        }
    }
}