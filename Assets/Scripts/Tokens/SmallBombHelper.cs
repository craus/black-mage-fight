using UnityEngine;
using System.Collections;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine.Events;
using System.Collections.Generic;

public class SmallBombHelper : MonoBehaviour
{
	public Mark bomb;
	public CellsColorChanger colorChanger;

	public void UpdateBoard() {
		Board.instance.cellsList.ForEach(c => {
			colorChanger.Unpaint(c);
		});
		Board.instance.cellsList.ForEach(c => {
			c.Figures.ForEach(f => {
				if (f.Marked(bomb)) {
					var timer = f.GetComponent<Counter>();
					var explosive = f.GetComponent<Explosive>();
					if (timer.value == 1) {
						explosive.ExplodeOnCells(false, cell => colorChanger.Paint(cell), wait => { });
					}
				}
			});
		});
	}
}
