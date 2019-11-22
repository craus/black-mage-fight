using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Linq;
using RSG;

public class LetterChecker : MonoBehaviour
{
	public List<Mark> letterMarks;
	public UnityEvent onSuccess;

	public bool diagonal = false;

	Promise check;

	List<Letter> CheckPosition(int x, int y, int index) {
		List<Letter> result = new List<Letter>();
		if (!Board.instance.Inside(x, y)) {
			return result;
		}
		var letterFigure = Board.instance.cells[x, y].figures.FirstOrDefault(f => f.Marked(letterMarks[index]));
		if (letterFigure == null) {
			return result;
		}
		var letter = letterFigure.GetComponent<Letter>();
		if (index == letterMarks.Count - 1) {
			result.Add(letter);
			return result;
		}
		result.AddRange(CheckPosition(x + 1, y, index + 1));
		result.AddRange(CheckPosition(x, y + 1, index + 1));
		result.AddRange(CheckPosition(x - 1, y, index + 1));
		result.AddRange(CheckPosition(x, y - 1, index + 1));
		if (diagonal) {
			result.AddRange(CheckPosition(x + 1, y + 1, index + 1));
			result.AddRange(CheckPosition(x - 1, y + 1, index + 1));
			result.AddRange(CheckPosition(x + 1, y - 1, index + 1));
			result.AddRange(CheckPosition(x - 1, y - 1, index + 1));
		}
		if (result.Count == 0) {
			result.Clear();
		} else {
			result.Add(letter);
		}
		return result;
	}

	List<Letter> CheckPosition(int x, int y) {
		return CheckPosition(x, y, 0);
	}

	public IPromise Check() {
		var result = new List<Letter>();
		for (int i = 0; i < Board.instance.cells.GetLength(0); i++) {
			for (int j = 0; j < Board.instance.cells.GetLength(1); j++) {
				result.AddRange(CheckPosition(i, j));
			}
		}
		return Promise.All(result.Select(l => l.Success()));
	}

	public IPromise Change() {
		if (check == null) {
			check = new Promise();
		}	
		return check;
	}

	public void Update() {
		if (check != null) {
			var checking = check;
			check = null;
			Check().Then(() => {
				checking.Resolve();
			});
		}
	}
}
