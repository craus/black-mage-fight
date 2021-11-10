using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;

public class Singletone<T> : MonoBehaviour where T : UnityEngine.Object {
	private static T _instance;

	public static T instance {
		get {
			if (_instance == null) {
				_instance = FindObjectOfType<T>();
			}
			return _instance;
		}
	}

	public virtual void OnDestroy() {
		//Debug.LogFormat("OnDestroy {0}", this);
		_instance = null;
	}
}
