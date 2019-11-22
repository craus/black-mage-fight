using UnityEngine;
using UnityEngine.UI;

public class LevelName : MonoBehaviour
{
	public static LevelName instance;

	public string levelName;

	public void Awake() {
		instance = this;
	}
}
