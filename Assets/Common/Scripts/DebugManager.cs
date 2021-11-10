using System.Collections;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using RSG;

public class DebugManager : Singletone<DebugManager>
{
	public bool slowAnimations;

    void Awake() {
        Promise.EnablePromiseTracking = true;
		Promise.DoNotHandleExceptions = true;
        Promise.UnhandledException += (object sender, ExceptionEventArgs e) => {
			Debug.LogErrorFormat("Unhandled exception from promises: {0}, {1}", sender, e.Exception);
			if (Promise.DoNotHandleExceptions) {
				throw e.Exception;
			}
        };
    }

    void Update() {
        if (Cheats.on) {
            if (Input.GetKeyDown(KeyCode.P)) {
                Debug.LogFormat("Pending promises: {0}", Promise.pendingPromises);
            }
            if (Input.GetKeyDown(KeyCode.S)) {
                slowAnimations ^= true;
                Debug.LogFormat("Slow animations: {0}", slowAnimations);
            }
            if (Input.GetKeyDown(KeyCode.F7)) {
                Debug.LogFormat(GameManager.instance.gameState.CurrentProfile.FullTextInfo);
            }
        }
    }
}
