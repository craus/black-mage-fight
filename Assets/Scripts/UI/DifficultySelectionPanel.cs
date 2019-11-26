using UnityEngine;
using System.Collections;
using UnityEngine.Audio;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class DifficultySelectionPanel : MonoBehaviour
{   
    public Toggle continuousRun;
    public Toggle panicMode;
    public Toggle randomized;
    public Toggle buildMode;
    public List<DifficultyButton> difficultyButtons;

    public void Go(int difficulty) {
        if (!GameManager.instance.gameState.CurrentProfile.Unlocked(GameLevels.instance.difficulties[difficulty])) {
            UI.instance.SoloConfirm("Пройди прежде предыдущий пункт");
            return;
        }
        var run = new GameRun();
        run.difficulty = difficulty;
        run.continuousRun = continuousRun.isOn;
        run.panicMode = panicMode.isOn;
        run.randomized = randomized.isOn;
        run.buildMode = buildMode.isOn;
        GameManager.instance.gameState.CurrentProfile.currentRuns.Add(run);
        GameManager.instance.Save();
		UI.instance.CloseAll();
        GameManager.instance.UpdateState();
    }

    public void Show() {
        gameObject.SetActive(true);
        difficultyButtons.ForEach(db => {
            db.gameObject.SetActive(GameManager.instance.gameState.CurrentProfile.Visible(db.difficulty));
            var colors = db.GetComponent<UnityEngine.UI.Button>().colors;
            colors.normalColor = GameManager.instance.gameState.CurrentProfile.Unlocked(db.difficulty) ? db.basic : db.locked;
            db.GetComponent<UnityEngine.UI.Button>().colors = colors;
            db.completedText.enabled = GameManager.instance.gameState.CurrentProfile.
				VictoryRuns().Any(cr => 
					cr.difficulty == db.difficulty.Value() &&
					cr.continuousRun == continuousRun.isOn &&
					cr.panicMode == panicMode.isOn
				);
        });
    }

	public void Update() {
		Show();
	}
}
