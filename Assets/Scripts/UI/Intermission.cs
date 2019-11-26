using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RSG;

public class Intermission : MonoBehaviour
{
    public static bool active = false;
    public bool ending = false;

    public List<GameObject> frames;
    Promise show;

    public IPromise Show() {
        gameObject.SetActive(true);
        active = true;
        frames.ForEach(f => f.gameObject.SetActive(false));
        frames.First().gameObject.SetActive(true);
        show = new Promise();
        return show;
    }

    public void Hide() {
        gameObject.SetActive(false);
        active = false;
    }

    void NextFrame() {  
        var currentFrame = frames.FirstOrDefault(f => f.activeSelf);
        if (currentFrame == null) {
            return;
        }
        if (currentFrame == frames.Last()) {
            Hide();
            show.Resolve();
        }
        currentFrame.SetActive(false);
		currentFrame = frames.CyclicNext(currentFrame);
        currentFrame.SetActive(true);
    }

    public void Update() {
		if (UI.instance.menu.gameObject.activeSelf) {
			return;
		}
        if (Input.anyKeyDown) {
            NextFrame();
        }
    }
}
