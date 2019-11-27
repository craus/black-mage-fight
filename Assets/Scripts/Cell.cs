using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

[ExecuteInEditMode]
public class Cell : MonoBehaviour {
    public int x, y;

    public List<Figure> figures = new List<Figure>();

    public List<Figure> Figures {
        get {
            return figures.Where(f => f.gameObject.activeInHierarchy).ToList();
        }
    }

    public MeshRenderer meshRenderer;

    Color baseColor;

    public void Init() {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public int Distance(Cell other) {
        return Mathf.Abs(x - other.x) + Mathf.Abs(y - other.y);
    }

	public int SquareEuclideanDistance(Cell other) {
		return this.sqr(x - other.x) + this.sqr(y - other.y);
	}

    void Awake() {
        Init();
        baseColor = meshRenderer.sharedMaterial.color;
    }

    public void ChangeColor(Color c) {
        meshRenderer.material.color = c;
    }

    public void RestoreColor() { 
        meshRenderer.material.color = baseColor;
    }

    public void MoveHere(Figure f) {
		//Debug.LogFormat("Figure {0} moves to cell {1}", f, this);
        f.transform.position = transform.position;
		var figuresAtMoveTime = figures.ShallowClone();
		var from = f.Position;
        f.SetPosition(this);
		if (from != null) {
			var leavingFiguresAtMoveTime = from.figures.ShallowClone();
			leavingFiguresAtMoveTime.ForEach(f2 => {
				if (f2.gameObject.activeSelf) {
					f2.Uncollide(f);
					f.Uncollide(f2);
				}
			});
		}
        figuresAtMoveTime.ForEach(f2 => {
			if (f2.gameObject.activeSelf) {
	            f2.Collide(f);
	            f.Collide(f2);
			}
        });
    }

	public Cell ToDirection(IntVector2 direction, int distance = 1) {
		return Board.instance.GetCell(x + direction.x * distance, y + direction.y * distance);
    }

	public Cell ToDirection(int dx, int dy, int distance = 1) {
		return Board.instance.GetCell(x + dx * distance, y + dy * distance);
	}

    public Cell Right() {
        return Board.instance.GetCell(x, y + 1);
    }
    public Cell Left() {
        return Board.instance.GetCell(x, y - 1);
    }
    public Cell Up() {
        return Board.instance.GetCell(x - 1, y);
    }
    public Cell Down() {
        return Board.instance.GetCell(x + 1, y);
    }
}
