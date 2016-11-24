﻿using UnityEngine;
using System.Collections;

public class Figure : MonoBehaviour
{
    public Cell position;

    public virtual void MoveTo(Cell cell) {
        if (cell == null) {
            return;
        }
        cell.MoveHere(this);
    }

    [ContextMenu("Place")]
    public void Place() {
        position.MoveHere(this);
    }

    public void Blink() {
        Board.instance.RandomEmptyCell().MoveHere(this);
    }
}
