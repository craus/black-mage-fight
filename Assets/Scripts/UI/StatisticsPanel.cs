using UnityEngine;
using System.Collections;
using UnityEngine.Audio;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class StatisticsPanel : MonoBehaviour
{   
	public Toggle otherRuns;
	public Toggle otherProfiles;

	public Text tries;
	public Text wins;
	public Text loses;
	public Text interrupts;
	public Text skips;
	public Text moves;
	public Text seconds;

	public Text title;

	public void Awake() {
		Hide();
	}

	public void Show() {
		gameObject.SetActive(true);
		UpdateTexts();
	}

	public void Hide() {
		gameObject.SetActive(false);
	}

	public void UpdateOtherRuns() {
		if (!otherRuns.isOn && otherProfiles.isOn) {
			otherProfiles.isOn = false;
		}
		UpdateTexts();
	}

	public void UpdateOtherProfiles() {
		if (!otherRuns.isOn && otherProfiles.isOn) {
			otherRuns.isOn = true;
		}
		UpdateTexts();
	}

	public IEnumerable<LevelRun> SelectByLevel(IEnumerable<LevelRun> levelRuns) {
		return levelRuns.Where(r => 
			r.levelID == GameManager.instance.currentLevel.gameObject.name &&
			r.difficulty == GameManager.instance.gameState.CurrentRun.difficulty &&
			r.panicMode == GameManager.instance.gameState.CurrentRun.panicMode &&
			r.continuousRun == GameManager.instance.gameState.CurrentRun.continuousRun
		);
	}

	public IEnumerable<LevelRun> ThisGameRun() {
		return SelectByLevel(GameManager.instance.gameState.CurrentRun.levelRuns);
	}

	public IEnumerable<LevelRun> ThisProfile() {
		return SelectByLevel(
			GameManager.instance.gameState.CurrentProfile.AllRuns()
				.SelectMany(gameRun => gameRun.levelRuns)
		);
	}

	public IEnumerable<LevelRun> AllProfiles() {
		return SelectByLevel(
			GameManager.instance.gameState.profiles.SelectMany(profile =>
				profile.AllRuns().SelectMany(gameRun => gameRun.levelRuns)
		));
	}

	public IEnumerable<LevelRun> Scope() {
		if (!otherRuns.isOn) {
			return ThisGameRun();
		}
		if (!otherProfiles.isOn) {
			return ThisProfile();
		}
		return AllProfiles();
	}

	public void UpdateTexts() {
		title.text = "{0}. {1}\n{2}".i(
			GameManager.instance.gameState.CurrentRun.DifficultyName(),
			GameManager.instance.currentLevel.LevelName(),
			GameManager.instance.gameState.CurrentRun.Modifiers().ExtToString(". ", "{0}")
		);
		var scope = Scope();
		tries.text = scope.Count().ToString();
		wins.text = scope.Count(r => r.blackMageHealth == 0).ToString();
		loses.text = scope.Count(r => r.blackMageHealth > 0 && r.steps >= 5).ToString();
		interrupts.text = scope.Count(r => r.blackMageHealth > 0 && r.heroHealth > 0).ToString();
		skips.text = scope.Count(r => r.blackMageHealth > 0 && r.heroHealth > 0 && r.steps < 5).ToString();
		if (scope.Count(r => r.blackMageHealth == 0) > 0) {
			moves.text = "{0:0.00}".i(scope.Where(r => r.blackMageHealth == 0).Average(r => r.steps));
			seconds.text = "{0:0.00}".i(scope.Where(r => r.blackMageHealth == 0).Average(r => r.seconds));
		} else {
			moves.text = "пусто";
			seconds.text = "пусто";
		}
	}
}
