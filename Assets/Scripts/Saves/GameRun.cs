using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;

[Serializable]
public class GameRun
{
    public bool continuousRun = false;
    public bool panicMode = false;
    public bool randomized = false;
    public bool buildMode = false;

	public bool interrupted = false;

    public int difficulty = -1;
    public int triesLeft = 5;
    public int levelsCompleted = 0;

	/// <summary>
	/// All attempts to play any level.
	/// </summary>
	public List<LevelRun> levelRuns = new List<LevelRun>();

	public string DifficultyName() {
		return GameLevels.instance.difficulties[difficulty].difficultyName;
	}

	public string CurrentLevelName() {
		return GameLevels.instance.commonLevels[levelsCompleted].LevelName();
	}

    public string Description() {
		if (Cheats.on) {
			return string.Format(
				"{0} – {1} ({2}/{3})", 
				DifficultyName(),
				GameLevels.instance.commonLevels[levelsCompleted].LevelName(),
				levelsCompleted + 1,
				GameLevels.instance.commonLevels.Count
			);
		} else {			
			return string.Format(
				"{0}. Уровень {1}. {2}", 
				DifficultyName(),
				levelsCompleted + 1,
				GameLevels.instance.commonLevels[levelsCompleted].LevelName()
			);
		}
    }

	public IEnumerable<string> Modifiers() {
		if (panicMode) {
			yield return "Приступ паники";
		}
		if (continuousRun) {
			yield return "Последовательное прохождение";
		}
	}
}
