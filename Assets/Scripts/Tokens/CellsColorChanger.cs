using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using RSG;
using System.Linq;
using System.Collections.Generic;
using System;

public class CellsColorChanger : MonoBehaviour
{
	public ColorSetter sample;
	Dictionary<Cell, ColorSetter> colorChangers = new Dictionary<Cell, ColorSetter>();

	public void Start() {
		Board.instance.cellsList.ForEach(c => {
			var provider = c.GetComponentInChildren<ColorPriorityValueProvider>();
			colorChangers[c] = Instantiate(sample);
			colorChangers[c].transform.SetParent(c.transform);
			provider.AddValueSetter(colorChangers[c]);
		});
	}

	public void Paint(Cell c) {
		colorChangers[c].Active = true;
	}

	public void Unpaint(Cell c) {
		if (!colorChangers.ContainsKey(c)) {
			return;
		}
		colorChangers[c].Active = false;
	}
}
