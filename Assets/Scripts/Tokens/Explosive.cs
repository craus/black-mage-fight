using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using RSG;
using System.Linq;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(Figure))]
public class Explosive : MonoBehaviour
{
    Figure figure;
    public GameObject explosionSample;
    public AudioSource explosionSound;
	public int squareRadius = 2;
	public float explosionSpriteRadius = 0.67f;

    public void Awake() {
        figure = GetComponent<Figure>();
    }

	IPromise PlayExplosionAnimation(Cell from) {
		var explosion = Instantiate(explosionSample);
		explosion.transform.localScale = Vector3.one * Mathf.Sqrt(squareRadius) * explosionSpriteRadius;
		explosion.SetActive(true);
		explosion.transform.position = from.transform.position;
		ExplosionArea().ForEach(cell => {
			cell.ChangeColor(Color.red);
		});

		return TimeManager.Wait(0.1f).Then(() => {
			Destroy(explosion);
			ExplosionArea().ForEach(cell => {
				cell.RestoreColor();
			});
		});
	}

	void CallExplosionListeners(Cell cell, bool actuallyExplode, Action<Cell> callback) {
        if (cell == null) {
            return;
        }
        if (cell.Figures.Count > 0) {
            cell.Figures.ForEach(f => {
                var onExplode = f.GetComponentInChildren<OnExplode>();
                if (onExplode) {
					onExplode.Run(actuallyExplode, callback);
                }
            });
        }
    }

	public List<Cell> ExplosionArea() {
		return Board.instance.cellsList.Where(
			c => c.SquareEuclideanDistance(figure.Position) <= squareRadius
		).ToList();
	}

	public void Explode() {
		ExplodeOnCells(true, c => {});
	}

    private void ExplodeInternal() {
		this.TryPlay(SoundManager.instance.explosion);

        gameObject.SetActive(false);
        Destroy(gameObject);

		PlayExplosionAnimation(figure.Position);
    }

	private bool explodingOnCellsRightNow = false;
	public void ExplodeOnCells(bool actuallyExplode, Action<Cell> callback) {
		if (!gameObject.activeSelf) {
			return;
		}
		if (explodingOnCellsRightNow) {
			return;
		}
		explodingOnCellsRightNow = true;

		if (actuallyExplode) {
			ExplodeInternal();
		}

		ExplosionArea().ForEach(cell => {
			callback(cell);
			CallExplosionListeners(cell, actuallyExplode, callback);
		});

		explodingOnCellsRightNow = false;
	}
}
