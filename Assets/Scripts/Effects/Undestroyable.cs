using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using RSG;
using UnityEngine.Events;

public class Undestroyable : MonoBehaviour
{
	public UnityEvent effect;

	public void Run() {
		transform.SetParent(SoundManager.instance.undestroyableSounds);
		effect.Invoke();
	}
}
