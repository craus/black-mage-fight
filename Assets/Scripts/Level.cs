using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class Level : MonoBehaviour {
    [ContextMenu("Run")] 
    public void Run() {
        GameManager.instance.RunLevel(this);
    }

	public string LevelName() {
		var levelNameScript = GetComponentInChildren<LevelName>();
		if (levelNameScript == null) {
			return gameObject.name;
		}
		return levelNameScript.levelName;
	}
}
