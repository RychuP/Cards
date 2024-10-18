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
    GameManager GameManager => Pile.GameManager;

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
        Hand.CardReceived += Hand_OnCardReceived;
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
        var animCard = GetCardGameComponent(e.Card);
        if (Visible) animCard.Visible = true;
    }
}