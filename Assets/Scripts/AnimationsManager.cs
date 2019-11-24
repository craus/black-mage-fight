using System.Collections;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using RSG;

public class AnimationsManager : MonoBehaviour
{
    public static AnimationsManager instance;

    IPromise currentAnimations = Promise.Resolved();

    public void Awake() {
        instance = this;
    }

	public IPromise RunAnimation(Func<IPromise> animation) {
        currentAnimations = currentAnimations.Then(animation);
		return currentAnimations;
    }

    public static IPromise Move(Transform t, Vector3 a, Vector3 b, float v) {
        Promise moved = new Promise();
        instance.StartCoroutine(MoveCoroutine(t,a,b, v, moved));
        return moved;
    }

    static IEnumerator MoveCoroutine(Transform t, Vector3 a, Vector3 b, float v, Promise moved) {
		if (DebugManager.instance.slowAnimations) {
			v /= 10;
		}
        Vector3 velocity = (b-a).normalized*v;
        float startTime = TimeManager.Time();
        float endTime = startTime + (b-a).magnitude / v;
        for (int i = 0; i < 1000; i++) {
            yield return null;
            float animationTime = Mathf.Clamp(TimeManager.Time(), startTime, endTime);
            t.position = a + velocity * (animationTime - startTime);
            if (animationTime == endTime) {
                moved.Resolve();
                yield break;
            }
        }
        moved.Reject(new Exception("Too long move"));
    }
}
