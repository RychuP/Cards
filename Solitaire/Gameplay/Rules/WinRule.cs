using Framework.Engine;
using Solitaire.Gameplay.Piles;
using Solitaire.Managers;

namespace Solitaire.Gameplay.Rules;

internal class WinRule : GameRule
{
    GameManager GameManager { get; }

    public WinRule(GameManager gm)
    {
        GameManager = gm;
    }

    public override void Check()
    {
        if (GameManager.Foundations.Find(f => f.Count != 13) is not null)
            return;
        else
            OnRuleMatch(EventArgs.Empty);
    }
}