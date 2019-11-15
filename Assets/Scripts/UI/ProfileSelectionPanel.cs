using UnityEngine;
using System.Collections;
using UnityEngine.Audio;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class ProfileSelectionPanel : MonoBehaviour
{   
    public List<UnityEngine.UI.Button> profileButtons;

    public Color basic;
    public Color selected;

    public void Show() {
        for (int i = 0; i < GameManager.instance.gameState.profiles.Count; i++) {
            var profile = GameManager.instance.gameState.profiles[i];
            profileButtons[i].GetComponentInChildren<Text>().text = profile.Description();
            var colors = profileButtons[i].GetComponentInChildren<UnityEngine.UI.Button>().colors;
            colors.normalColor = GameManager.instance.gameState.CurrentProfile == profile ? selected : basic;
            profileButtons[i].GetComponentInChildren<UnityEngine.UI.Button>().colors = colors;
        }
        gameObject.SetActive(true);
    }

	public void RemoveProfile(int id) {
		GameManager.instance.gameState.profiles[id] = new Profile();
		GameManager.instance.Save();
		GameManager.instance.UpdateState();
	}

    public void SelectProfile(int id) {
        if (Input.GetKey(KeyCode.LeftShift)) {
			RemoveProfile(id);
			return;
        }
        if (GameManager.instance.gameState.currentProfileIndex == id) {
            UI.instance.CloseAll();
            return;
        }
        GameManager.instance.gameState.currentProfileIndex = id;
        GameManager.instance.Save();
        GameManager.instance.UpdateState();
    }
}
