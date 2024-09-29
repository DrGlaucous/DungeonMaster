using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonMaster
{
    public delegate void StringDelegate(string argument);

    public delegate void VoidDelegate();

    public delegate void IntDelegate(int argument);

    public delegate void TimespanDelegate(TimeSpan argument);

    public delegate void ScoreboardActionDelegate(Scoreboard.ScoreboardAction action, params object[] list);
}
