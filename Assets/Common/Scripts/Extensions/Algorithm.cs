using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

public static class Algorithm
{
	public static float BinarySearch(float min, float max, Func<float, bool> bigEnough) {
		var result = min;
		var step = max - min;
		while (step > float.Epsilon) {
			if (!bigEnough(result + step)) {
				result += step;
			}
			step /= 2;
		}
		return result;
	}
}