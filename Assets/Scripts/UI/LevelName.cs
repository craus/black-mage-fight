using UnityEngine;
using UnityEngine.UI;

public class LevelName : MonoBehaviour
{
	public Text text;

	public void Update() {
		if (GameManager.instance.gameState.CurrentRun == null) {
			return;
		}
		text.text = string.Format(
			"{0} – {1} ({2}/{3})", 
			GameLevels.instance.difficulties[GameManager.instance.gameState.CurrentRun.difficulty].name,
			GameManager.instance.currentLevel.name,
			GameManager.instance.gameState.CurrentRun.levelsCompleted,
			GameLevels.instance.commonLevels.Count
		);
	}

	public void Awake() {
		text = GetComponent<Text>();
	}
}
