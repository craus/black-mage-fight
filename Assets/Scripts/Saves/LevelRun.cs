using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;

[Serializable]
public class LevelRun
{
	public bool continuousRun = false;
	public bool panicMode = false;
	public int triesLeft = 5;

	public string levelID;
	public int difficulty;
	public int steps;
	public int seconds;
	public int blackMageHealth;
	public int heroHealth;



	public string Description() {
		string result = string.Format("{0}-{1}{6}{7} {2}:{3} ({4} steps, {5} sec)",
			levelID,
			difficulty+1,
			heroHealth,
			blackMageHealth,
			steps,
			seconds,
			panicMode ? "P" : "",
			continuousRun ? "C({0})".i(triesLeft) : ""
		);
		if (continuousRun) {
			result += "\n" + triesLeft;
		}
		return result;
	}
}
