using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System;
using RSG;

public class OnExplode : MonoBehaviour
{
	public UnityEvent run;
	public BoolActionCellActionIPromiseUnityEvent runOnCells;

	public void Run(
		bool actuallyExplode = true, 
		Action<Cell> callback = null,
		Action<IPromise> callbackWait = null
	) {
		callback = callback ?? (c => {});
		callbackWait = callbackWait ?? (c => { });
		if (actuallyExplode) {
			run.Invoke();
		}
		runOnCells.Invoke(actuallyExplode, callback, callbackWait);
	}
}
