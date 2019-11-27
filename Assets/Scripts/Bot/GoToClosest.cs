using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class GoToClosest : MonoBehaviour
{
	const int N = 8;
	const int N1 = N - 1;
	const int N2 = N / 2;
	const int S = N * N;

	public bool dynamicBlackMageCost = true;

	public double blackMageCost = 2.5;
	public double heartCost = 3;
	public double distanceFromCenterCost = 1e-9;
	public double distanceToBonusCost = 1;
	public double randomCost = 0;

	public int indexToLook;
	public int stepCount = 1;

	public bool on = false;
	public bool logging = true;

	public Cell target;
	public Cell distantTarget;

	public Map<Cell, Map<Cell, Algorithm.Weighted<Cell>>> shortestPaths = new Map<Cell, Map<Cell, Algorithm.Weighted<Cell>>>();

	double[] ClearMatrix() {
		return new double[S / 4 * S * S * S * 4]; // hero, blackMage/1st heart, blackMage/2nd heart, blackMage/3rd heart, blackMageIndex
		// heroX = 0..3, heroY = 0..3, heroX <= heroY
		// blackMageIndex = 0..3 (a,b,c,d)
	}

	void swap(ref int a, ref int b) {
		a ^= b;
		b ^= a;
		a ^= b;
	}

	public bool IsBonus(Figure f) {
		if (f is BlackMage) {
			return true;
		}
		if (f is Heart) {
			return true;
		}
		if (f is Antidote) {
			return true;
		}
		if (f.GetComponent<Statue>() != null) {
			return true;
		}
		if (f.Marked(Marks.Bomb, Marks.BlackMageRelocator)) {
			return true;
		}
		return false;
	}

	public IEnumerable<Figure> Bonuses() {
		return Board.instance.cellsList.SelectMany(c => c.Figures.Where(IsBonus));
	}

	public Algorithm.Weighted<Cell> Edge(Cell to) {
		float weight = 1;
		var skull = to.Figures.FirstOrDefault(f => f is Skull) as Skull;
		if (skull != null) {
			weight += skull.damage;
		}
		return new Algorithm.Weighted<Cell>(to, weight);
	}

	public IEnumerable<Cell> Neighbours(Cell from) {
		yield return from.ToDirection(1, 0);
		yield return from.ToDirection(-1, 0);
		yield return from.ToDirection(0, 1);
		yield return from.ToDirection(0, -1);
	}

	public IEnumerable<Algorithm.Weighted<Cell>> Edges(Cell from) {
		foreach (Cell c in Neighbours(from)) {
			if (c != null) {
				yield return Edge(c);
			}
		}
	}

	public Map<Cell, Algorithm.Weighted<Cell>> ShortestPaths(Cell from) {
		if (!shortestPaths.ContainsKey(from)) {
			shortestPaths[from] = Algorithm.Dijkstra(from, Edges);
		}
		return shortestPaths[from];
	}

	public float Distance(Figure a, Figure b) {

		return ShortestPaths(a.Position)[b.Position].weight;
	}

	public Cell StepTo(Cell target) {
		var paths = ShortestPaths(Hero.instance.Position);
		for (int i = 0; i < 100; i++) {
			if (paths[target].to == Hero.instance.Position) {
				break;
			}
			target = paths[target].to;
		}
		return target;
	}

	public void Move() {
		shortestPaths.Clear();
		if (Hero.instance.health < 1 || BlackMage.instance.health < 1) {
			Controls.instance.Restart();
			return;
		}
		if (dynamicBlackMageCost) {
			blackMageCost = 1.0 * Hero.instance.health / BlackMage.instance.health;
		}
		distantTarget = Bonuses().MinBy(f => Distance(Hero.instance, f)).Position;
		target = StepTo(distantTarget);
		if (target.x > Hero.instance.Position.x) {
			Controls.instance.down.Press();
		} else if (target.x < Hero.instance.Position.x) {
			Controls.instance.up.Press();
		} else if (target.y > Hero.instance.Position.y) {
			Controls.instance.right.Press();
		} else if (target.y < Hero.instance.Position.y) {
			Controls.instance.left.Press();
		} else {
		}
	}

	public void Update() {
		if (Input.GetKeyDown(KeyCode.M)) {
			Debug.LogFormat("GoToClosest.Move");
			Move();
		}
		if (Input.GetKeyDown(KeyCode.A)) {
			Debug.LogFormat("GoToClosest.Autoplay = {0}", on);
			on ^= true;
		}
		if (on) {
			Move();
		}
	}
}
