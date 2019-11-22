using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using System;

[Serializable]
public class BoolActionCellUnityEvent : UnityEvent<bool, Action<Cell>> {
}
