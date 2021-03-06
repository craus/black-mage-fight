﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using RSG;
using System.Linq;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(Figure))]
public class Explosive : MonoBehaviour
{
	public CellsColorChanger colorChanger;

    Figure figure;
    public GameObject explosionSample;
    public AudioSource explosionSound;
	public int squareRadius = 2;

	public IntValueProvider squareRadiusProvider;

	public int SquareRadius {
		get {
			if (squareRadiusProvider != null) {
				return squareRadiusProvider.Value;
			}
			return squareRadius;
		}
	}

	public float explosionSpriteRadius = 0.67f;

    public void Awake() {
        figure = GetComponent<Figure>();
    }

	IPromise PlayExplosionAnimation(Cell from) {
		var explosion = Instantiate(explosionSample);
		explosion.transform.localScale = Vector3.one * Mathf.Sqrt(SquareRadius) * explosionSpriteRadius;
		explosion.SetActive(true);
		explosion.transform.position = from.transform.position;

		var area = ExplosionArea();

		if (colorChanger != null) {
			area.ForEach(cell => {
				colorChanger.Paint(cell);
			});
		}

		return TimeManager.Wait(0.1f).Then(() => {
			Destroy(explosion);
			if (colorChanger != null) {
				area.ForEach(cell => {
					colorChanger.Unpaint(cell);
				});
			}
		});
	}

	void CallExplosionListeners(
		Cell cell, 
		bool actuallyExplode, 
		Action<Cell> callbackCell,
		Action<IPromise> callbackWait
	) {
        if (cell == null) {
            return;
        }
        if (cell.Figures.Count > 0) {
            cell.Figures.ForEach(f => {
                var onExplode = f.GetComponentInChildren<OnExplode>();
                if (onExplode) {
					onExplode.Run(actuallyExplode, callbackCell, callbackWait);
                }
            });
        }
    }

	public List<Cell> ExplosionArea() {
		return Board.instance.cellsList.Where(
			c => c.SquareEuclideanDistance(figure.Position) <= SquareRadius
		).ToList();
	}

	public void LongExplode(Action<IPromise> callback) {
        ExplodeOnCells(true, c => { }, callback, false);
	}

	public void Explode() {
		ExplodeOnCells(true, c => { }, wait => { }, false);
	}

	private IPromise ExplodeInternal() {
		this.TryPlay(SoundManager.instance.explosion);

        gameObject.SetActive(false);
        Destroy(gameObject);

		return PlayExplosionAnimation(figure.Position);
    }

	public void ExplodeOnCells(bool actuallyExplode, Action<Cell> explodeCell, Action<IPromise> wait) {
		ExplodeOnCells(actuallyExplode, explodeCell, wait, true);
	}

	private bool explodingOnCellsRightNow = false;
	private bool goingToExplode = false;
	public void ExplodeOnCells(bool actuallyExplode, Action<Cell> explodeCell, Action<IPromise> wait, bool delay) {
		if (!gameObject.activeSelf || goingToExplode) {
			return;
		}
		if (explodingOnCellsRightNow) {
			return;
		}
		explodingOnCellsRightNow = true;
		if (actuallyExplode) {
			goingToExplode = true;
		}

		IPromise explosionFinished = (actuallyExplode && delay ? TimeManager.Wait(0.01f) : Promise.Resolved()).Then(() => {
			var exploded = actuallyExplode ? ExplodeInternal() : Promise.Resolved();
			ExplosionArea().ForEach(cell => {
				explodeCell(cell);
				CallExplosionListeners(cell, actuallyExplode, explodeCell, wait);
			});
			exploded.Done();
			//return exploded;
		});
		//explosionFinished.Done();
		wait(explosionFinished);

		explodingOnCellsRightNow = false;
	}
}
