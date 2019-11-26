using UnityEngine;
using System.Collections;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MenuPanel : MonoBehaviour
{   
    public Text profileDescription;
	public GameObject cheatMenu;
	public GameObject cheatButton;

	public void Awake() {
		cheatMenu.SetActive(false);
	}

    public void SwitchProfile() {
        GameManager.instance.gameState.currentProfileIndex = -1;
        GameManager.instance.Save();
        GameManager.instance.UpdateState();
    }

	public void StartCheating() {
		cheatMenu.SetActive(true);
		cheatButton.SetActive(false);
	}

	public void StopCheating() {
		cheatMenu.SetActive(false);
		cheatButton.SetActive(true);
	}

    public void InterruptRun() {
        UI.instance.Confirm("Прохождение потеряется полностью. Продолжить?").Then(() => {
            GameManager.instance.gameState.CurrentProfile.currentRuns.Clear();
			GameManager.instance.Save();
			UI.instance.CloseAll();
            GameManager.instance.UpdateState();
        });
    }

    public void Exit() {
        UI.instance.Confirm("Прощаемся?").Then(() => {
            Debug.LogFormat("Quit");
            Application.Quit();    
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #endif
        });
    }

    public void Show() {
        profileDescription.text = GameManager.instance.gameState.CurrentProfile.Description();
        gameObject.SetActive(true);
    }
}
