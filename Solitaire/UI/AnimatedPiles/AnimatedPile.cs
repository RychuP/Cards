using Framework.Engine;
using Framework.Misc;
using Framework.UI;
using Microsoft.Xna.Framework.Graphics;
using Solitaire.Gameplay.Piles;
using Solitaire.Managers;
using Solitaire.Misc;
using Solitaire.UI.Screens;

namespace Solitaire.UI.AnimatedGameComponents;

internal class AnimatedPile : AnimatedHandGameComponent
{
    public Pile Pile { get; }
    protected TraditionalCard DraggedCard { get; private set; }
    protected GameManager GameManager => Pile.GameManager;

    public AnimatedPile(Pile pile) : base((int)pile.Place, pile, pile.GameManager)
    {
        Pile = pile;

        // calculate position
        float offsetX = (Pile.OutlineWidth - TraditionalCard.Width) / 2;
        float offsetY = (Pile.OutlineHeight - TraditionalCard.Height) / 2;
        Position = pile.Position + new Vector2(offsetX, offsetY);

        // add itselft hidden to game components
        GameManager.Game.Components.Add(this);
        Hide();

        // register event handlers
        var gameplayScreen = GameManager.ScreenManager.GetScreen<GameplayScreen>();
        gameplayScreen.VisibleChanged += GameplayScreen_OnVisibleChanged;
        GameManager.InputManager.LeftMouseButtonDown += InputManager_OnLeftMouseButtonDown;
        GameManager.InputManager.LeftMouseButtonReleased += InputManager_OnLeftMouseButtonReleased;
        Hand.CardReceived += Hand_OnCardReceived;
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        var im = GameManager.InputManager;
        if (im.IsDragging && DraggedCard != null)
        {
            var offset = im.MousePosition - im.MouseDownInitialPosition;
            for (int i = Hand.Count - 1; i >= 0; i--)
            {
                var card = Hand[i];
                var animCard = GetCardGameComponent(card);
                var position = animCard.Position.ToPoint() + offset;
                animCard.Destination = new Rectangle(position, TraditionalCard.Size);
                var test = animCard.DrawOrder;
                if (card == DraggedCard)
                    return;
            }
        }
    }

    public override void Draw(GameTime gameTime)
    {
        if (Pile.OutlineVisible)
        {
            var sb = Game.Services.GetService<SpriteBatch>();
            sb.Begin();
            sb.Draw(Art.CardOutline, Pile.Position, Color.White);
            sb.End();
        }

        base.Draw(gameTime);
    }

    public override Vector2 GetCardRelativePosition(int cardLocationInHand) =>
        Vector2.Zero;

    public void Show()
    {
        Visible = true;
        foreach (var animCard in AnimatedCards)
            animCard.Visible = true;
    }

    public void Hide()
    {
        Visible = false;
        foreach (var animCard in AnimatedCards)
            animCard.Visible = false;
    }

    void GameplayScreen_OnVisibleChanged(object o, EventArgs e)
    {
        if (o is not GameplayScreen gameplayScreen) return;
        if (gameplayScreen.Visible == true)
            Show();
        else
            Hide();
    }

    protected virtual void Hand_OnCardReceived(object o, CardEventArgs e)
    {
        // make the cards visible (especially important during gameplay)
        var animCard = GetCardGameComponent(e.Card);
        animCard.Visible = Visible;
    }

    protected virtual void InputManager_OnLeftMouseButtonDown(object o, PointEventArgs e)
    {
        if (Pile.Bounds.Contains(e.Position) && Hand.Count > 0)
        {
            DraggedCard = GetCardFromPosition(e.Position);

            
            if (DraggedCard != null)
            {
                // remove and add cards from the game components to adjust their draw order
                // so when they are dragged they don't fall below other cards
                bool cardFound = false;
                for (int i = 0; i < Hand.Count; i++)
                {
                    var card = Hand[i];
                    if (!cardFound)
                        cardFound = card == DraggedCard;
                    
                    if (cardFound)
                    {
                        var animCard = GetCardGameComponent(card);
                        Game.Components.Remove(animCard);
                        Game.Components.Add(animCard);
                    }
                }
            }
        }
    }

    protected virtual void InputManager_OnLeftMouseButtonReleased(object o, PointEventArgs e)
    {
        // this needs work... TODO
        if (DraggedCard != null)
        {
            // reset drawing destinations
            foreach (var animCard in AnimatedCards)
                animCard.Destination = null;

            // find pile where the left mouse button was released
            var destPile = GameManager.GetPileFromPosition(e.Position);
            if (destPile is not null && destPile != Pile)
                Pile.DropCards(destPile, DraggedCard);

            DraggedCard = null;
        }
    }

    /// <summary>
    /// Returns the card whose visible part contains the given position.
    /// </summary>
    /// <param name="position">Usually the position from the mouse click.</param>
    public virtual TraditionalCard GetCardFromPosition(Point position) => null;
}