using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class SpawnerMarker<T> : OptionalSingletone<T> where T : MonoBehaviour 
{
	public PeriodicCounter periodicCounter;

	public List<PeriodicCounter> periodicCounters;

	public override void Awake() {
		ExistByCondition.AwakeAll(GameManager.instance.currentLevel);
		base.Awake();
		periodicCounter = GetComponentInChildren<PeriodicCounter>();
		periodicCounters = GetComponentsInChildren<PeriodicCounter>().ToList();
		Debug.LogFormat("Got periodic counters");
	}
}
