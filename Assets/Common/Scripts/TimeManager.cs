using System.Collections;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using RSG;

public class TimeManager : Singletone<TimeManager>
{
	PromiseTimer promiseTimer = new PromiseTimer();

    float lastUpdateTime = 0;

    public static float Time() {
        return UnityEngine.Time.time;
    }

    public void Update() {
        promiseTimer.Update(Time() - lastUpdateTime);
        lastUpdateTime = Time();
    }

    public static IPromise Wait(float seconds) {
		if (DebugManager.instance.slowAnimations) {
			seconds *= 10;
		}
        return instance.promiseTimer.WaitFor(seconds);
    }
}
