using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Linq;
using RSG;

public class LetterLine : MonoBehaviour
{
	public Letter letter;
	public List<LineRenderer> lines;
	public LetterChecker checker;

	public void Awake() {
		checker = FindObjectOfType<LetterChecker>();
	}

	public void Start() {
		Controls.instance.onUnlocked += Run;
		Run();
	}

	void Check(int dx, int dy, int lineIndex) {
		var cell = letter.figure.Position.ToDirection(new IntVector2(dx, dy));
		if (cell == null) {
			return;
		}
		var index = checker.letterMarks.IndexOf(letter.marker.mark)+1;
		if (index >= checker.letterMarks.Count) {
			return;
		}
		var nextLetter = cell.figures.FirstOrDefault(f => f.Marked(checker.letterMarks[index]));
		if (nextLetter == null) {
			return;
		}
		lines[lineIndex].enabled = true;
		lines[lineIndex].SetPosition(1, cell.transform.position - letter.transform.position);
	}

	public void Run() {
		lines.ForEach(l => l.enabled = false);
		Check(1, 0, 0);
		Check(0, 1, 1);
		Check(-1, 0, 2);
		Check(0, -1, 3);
		if (checker.diagonal) {
			Check(1, 1, 4);
			Check(1, -1, 5);
			Check(-1, 1, 6);
			Check(-1, -1, 7);
		}
	}

	void OnDestroy() {
		if (Controls.instance) {
			Controls.instance.onUnlocked -= Run;
		}
	}
}
