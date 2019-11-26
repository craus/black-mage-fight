using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

[Serializable]
public class Profile
{
    public string name = "";
	public bool skipIntros = false;

	/// <summary>
	/// All game runs which are not current
	/// </summary>
    public List<GameRun> completedRuns = new List<GameRun>();

	/// <summary>
	/// List of current game runs. For now, contains 0 or 1 element.
	/// </summary>
    public List<GameRun> currentRuns = new List<GameRun>();

	public IEnumerable<GameRun> VictoryRuns() {
		return completedRuns.Where(r => r.levelsCompleted >= GameLevels.instance.commonLevels.Count);
	}

	public IEnumerable<GameRun> AllRuns() {
		return completedRuns.Concat(currentRuns);
	}

    public string Description() {
        if (name == "") {
            return String.Format("<b><size=24>Пусто</size></b>");
        }
        return String.Format("<b><size=24>{0}</size></b>{1}", name, currentRuns.Count == 0 ? "" : "\n"+currentRuns[0].Description());
    }

    public bool Unlocked(Difficulty d) {
        var diff = d.Value();
        if (diff <= 2) {
            return true;
        }
		return VictoryRuns().Any(cr => cr.difficulty >= diff - 1);
    }

    public bool Visible(Difficulty d) {
        var diff = d.Value();
        if (diff <= 3) {
            return true;
        }
        var delta = 2;
        if (diff == 5) {
            delta = 1;
        }
		return VictoryRuns().Any(cr => cr.difficulty >= diff - delta);
    }
}
