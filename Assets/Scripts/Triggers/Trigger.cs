using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System;
using RSG;
using System.Collections.Generic;

public class Trigger : MonoBehaviour
{
    public UnityEvent effect;
	public ActionPromiseUnityEvent longEffect;

    public virtual void Run() {
		if (!gameObject.activeInHierarchy) {
			return;
		}
        effect.Invoke();
    }

	public virtual IPromise LongRun() {
		if (!gameObject.activeInHierarchy) {
			return Promise.Resolved();
		}
		return Waiter.Wait(longEffect.Invoke);
	}
}
