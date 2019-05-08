﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using RSG;

public class UndestroyableSound : MonoBehaviour
{
	public AudioSource audioSource;

	public void Awake() {
		audioSource = GetComponent<AudioSource>();
	}

	public void Play() {
		transform.SetParent(SoundManager.instance.undestroyableSounds);
		audioSource.Play();
	}
}
