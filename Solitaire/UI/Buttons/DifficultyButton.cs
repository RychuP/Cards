using Solitaire.Managers;

namespace Solitaire.UI.Buttons;

internal class DifficultyButton : Button
{
    public DifficultyButton(GameManager gm) : base(gm.Difficulty.ToString(), gm.Game)
    {
        gm.DifficultyChanged += GameManager_OnDifficultyChanged;
    }

    void GameManager_OnDifficultyChanged(object o, EventArgs e)
    {
        if (o is not GameManager gm) return;
        Text = gm.Difficulty.ToString();
    }
}