using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using System;
using RSG;

[Serializable]
public class ActionPromiseUnityEvent : UnityEvent<Action<IPromise>>
{
}
