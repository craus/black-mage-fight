using UnityEngine;
using UnityEngine.UI;

public class LevelNameDisplay : Singletone<LevelNameDisplay>
{
	public Text text;

	public void Update() {
		if (GameManager.instance.gameState.CurrentRun == null) {
			return;
		}
		text.text = GameManager.instance.gameState.CurrentRun.Description();
	}

	public void Awake() {
		text = GetComponent<Text>();
	}
}
