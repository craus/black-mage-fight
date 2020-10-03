using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HeartStopperPeriodic : OptionalSingletone<HeartStopperPeriodic>
{
	public PeriodicCounter periodicCounter;

    public override void Awake() {
        base.Awake();
        periodicCounter = GetComponent<PeriodicCounter>();
    }
}
