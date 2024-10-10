using Framework.Engine;
using System.Collections.Generic;

namespace Poker.Gameplay.Evaluation;

internal class CardValueArrayComparer : IComparer<TraditionalCard[]>
{
    public int Compare(TraditionalCard[] x, TraditionalCard[] y)
    {
        return x[0].CompareTo(y[0]);
    }
}