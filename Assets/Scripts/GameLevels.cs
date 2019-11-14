using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class GameLevels : MonoBehaviour {
    public List<Difficulty> difficulties;

    public List<Level> commonLevels;

    public GameObject commonObjects;

	public Transform commonLevelsFolder;

    public static GameLevels instance;

	void FindCommonLevels() {
		commonLevels = commonLevelsFolder.Children().Select(c => c.GetComponent<Level>()).ToList();
	}

    void Awake() {
        instance = this;
		FindCommonLevels();
        transform.Children().ForEach(c => c.gameObject.SetActive(false));
    }

	[ContextMenu("Write Levels")]
	public void WriteLevelsList() {
		FindCommonLevels();
		Debug.LogFormat("Levels:\n{0}", commonLevels.ExtToString("\n", "{0}", x => x.name));
	}
}
