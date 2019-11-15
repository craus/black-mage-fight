using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DifficultyScaled : IntValueProvider
{
	public List<int> values;

	public override int Value {
		get {
			if (GameManager.instance.gameState.CurrentRun == null) {
				return values[0];
			}
			return values[GameManager.instance.gameState.CurrentRun.difficulty];
		}
	}
}
