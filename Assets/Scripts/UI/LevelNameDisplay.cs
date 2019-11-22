using UnityEngine;
using UnityEngine.UI;

public class LevelNameDisplay : Singletone<LevelNameDisplay>
{
	public Text text;

	public void Update() {
		if (GameManager.instance.gameState.CurrentRun == null) {
			return;
		}
		string levelName = GameManager.instance.currentLevel.name;
		if (LevelName.instance != null) {
			levelName = LevelName.instance.levelName;
		}
		if (Cheats.on) {
			text.text = string.Format(
				"{0} – {1} ({2}/{3})", 
				GameLevels.instance.difficulties[GameManager.instance.gameState.CurrentRun.difficulty].name,
				levelName,
				GameManager.instance.gameState.CurrentRun.levelsCompleted + 1,
				GameLevels.instance.commonLevels.Count
			);
		} else {			
			text.text = string.Format(
				"{0}. Уровень {1}. {2}", 
				GameLevels.instance.difficulties[GameManager.instance.gameState.CurrentRun.difficulty].name,
				GameManager.instance.gameState.CurrentRun.levelsCompleted + 1,
				levelName
			);
		}
	}

	public void Awake() {
		text = GetComponent<Text>();
	}
}
