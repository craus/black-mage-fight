using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using System;
using RSG;

[Serializable]
public class BoolActionCellActionIPromiseUnityEvent
	: UnityEvent<bool, Action<Cell>, Action<IPromise>>
{
}
