using System;
using UnityEngine;

public class ExistByCondition : MonoBehaviour
{
	public static void AwakeAll(MonoBehaviour parent) {
		parent.GetComponentsInChildren<ExistByCondition>().ForEach(e => e.Awake()); // remove non-existant onlevelStart triggers first
	}

    public Condition condition;

    public void Awake() {
        if (!condition.Satisfied()) {
			gameObject.SetActive(false);
            Destroy(gameObject);
			Debug.LogFormat("Destroyed non-existent");
        }
    }
}

