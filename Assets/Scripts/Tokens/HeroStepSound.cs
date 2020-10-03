using UnityEngine;
using System.Collections;
using System.Linq;
using System;

public class HeroStepSound : MonoBehaviour
{
	public AudioSource sound;
	public KeyCounter keyCounter;

	public void Awake() {
		sound = GetComponent<AudioSource>();
		keyCounter = FindObjectOfType<KeyCounter>();
	}

	public float Pitch() {
		if (DoorSpawner.instance) {
			return keyCounter.counter.value > 0 ? 2 : 1;
		}
		return 1;
	}

	public void Update() {
		var newPitch = Pitch();
		if (sound.pitch != newPitch) {
			sound.pitch = newPitch;
		}
	}
}
