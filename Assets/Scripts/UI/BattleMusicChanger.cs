using UnityEngine;
using System.Collections;
using UnityEngine.Audio;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class BattleMusicChanger : Singletone<BattleMusicChanger>
{   
	public List<AudioSource> musics;

	public AudioSource current;

	public void Update() {
		var run = GameManager.instance.gameState.CurrentRun;
		if (run == null) {
			return;
		}
		int musicIndex = Mathf.Clamp(run.levelsCompleted * musics.Count / GameLevels.instance.commonLevels.Count, 0, musics.Count-1);
		if (current != musics[musicIndex]) {
			if (current != null) {
				current.Stop();
			}
			current = musics[musicIndex];
			current.Play();
		}
	}
}
