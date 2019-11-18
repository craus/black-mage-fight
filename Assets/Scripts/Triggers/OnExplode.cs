using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System;

public class OnExplode : MonoBehaviour
{
	public UnityEvent run;
	public BoolActionCellUnityEvent runOnCells;

	public void Run(bool actuallyExplode = true, Action<Cell> callback = null) {
		callback = callback ?? (c => {});
		if (actuallyExplode) {
			run.Invoke();
		}
		runOnCells.Invoke(actuallyExplode, callback);
	}
}
