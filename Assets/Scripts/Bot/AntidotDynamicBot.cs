using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class AntidotDynamicBot : MonoBehaviour
{
	double[] a = null;
	int step = 0;

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

	string filename = "antidotBotData.dat";

	public Cell target;

	double[] ClearMatrix() {
		return new double[S / 4 * S * S * S * 4]; // hero, blackMage/1st heart, blackMage/2nd heart, blackMage/3rd heart, blackMageIndex
		// heroX = 0..3, heroY = 0..3, heroX <= heroY
		// blackMageIndex = 0..3 (a,b,c,d)
	}

	void Awake() {
		//Load();
	}

	[ContextMenu("Generate")]
	public void Generate() {
		a = ClearMatrix();
		Save();
	}

	[ContextMenu("Save")]
	public void Save() {
		FileManager.SaveToFile(a, filename);
		Debug.LogFormat("a saved");
		PrintInfo();
	}

	[ContextMenu("Load")]
	public void Load() {
		a = FileManager.LoadFromFile<double[]>(filename);
		Debug.LogFormat("a loaded");
		PrintInfo();
	}

	[ContextMenu("Print an info")]
	public void PrintInfo() {
		Debug.LogFormat("a hash = {0}", Hash());
		int cnt = 0;
		int cntZero = 0;
		for (int i = 0; i < (1 << 24); i++) {
			if (!double.IsNaN(a[i])) {
				++cnt;
			}
			if (Math.Abs(a[i]) < 0.001) {
				++cntZero;
			}
		}
		Debug.LogFormat("numbers in a: {0}", cnt);
		Debug.LogFormat("zeroes in a: {0}", cntZero);
	}

	[ContextMenu("Look at index")]
	public void LookAtIndex() {
		Debug.LogFormat("a[{0}] = {1:0.####}", indexToLook, a[indexToLook]);
	}

	[ContextMenu("Test")]
	public void Test() {
		int x = 5;
		int y = 7;
		swap(ref x, ref y);
		Debug.LogFormat("x = {0}, y = {1}", x, y);
	}

	public int Hash() {
		int hash = 0;
		for (int i = 0; i < (1 << 24); i++) {
			hash = hash * 31 + a[i].GetHashCode();
		}
		return hash;
	}


	void swap(ref int a, ref int b) {
		a ^= b;
		b ^= a;
		a ^= b;
	}

	// desicion = 0..3 (a,b,c,d)
	double GetResult(int heroX, int heroY, int ax, int ay, int bx, int by, int cx, int cy, int dx, int dy, int blackMageIndex, int decision, bool debug = false) {
		if (a == null) {
			Load();
		}
		//if (debug) {
		//    Debug.LogFormat("GetResult(heroX = {0}, heroY = {1}, ax = {2}, ay = {3}, bx = {4}, by = {5}, cx = {6}, cy = {7}, dx = {8}, dy = {9}, blackMageIndex = {10}, decision = {11}",
		//        heroX, heroY, ax, ay, bx, by, cx, cy, dx, dy, blackMageIndex, decision);
		//}
		double result = 0;
		if (decision == blackMageIndex) {
			result += blackMageCost;
		} else {
			result += heartCost;
		}
		int targetX = -1, targetY = -1;
		if (decision == 0) {
			targetX = ax; targetY = ay;
			ax = dx; ay = dy; if (blackMageIndex == 3) blackMageIndex = 0;
		}
		if (decision == 1) {
			targetX = bx; targetY = by;
			bx = dx; by = dy; if (blackMageIndex == 3) blackMageIndex = 1;
		}
		if (decision == 2) {
			targetX = cx; targetY = cy;
			cx = dx; cy = dy; if (blackMageIndex == 3) blackMageIndex = 2;
		}
		if (decision == 3) {
			targetX = dx; targetY = dy;
		}
		result -= (Math.Abs(heroX - targetX) + Math.Abs(heroY - targetY)) * distanceToBonusCost;
		heroX = targetX;
		heroY = targetY;
		result -= distanceFromCenterCost * (Math.Abs(3.5 - heroX) + Math.Abs(3.5 - heroY));
		if (heroX >= 4) {
			heroX = N1 - heroX; ax = N1 - ax; bx = N1 - bx; cx = N1 - cx;
		}
		if (heroY >= 4) {
			heroY = N1 - heroY; ay = N1 - ay; by = N1 - by; cy = N1 - cy;
		}
		if (heroX > heroY) {
			swap(ref heroX, ref heroY);
			swap(ref ax, ref ay);
			swap(ref bx, ref by);
			swap(ref cx, ref cy);
		}
		if (bx < ax || bx == ax && by <= ay) {
			swap(ref ax, ref bx);
			swap(ref ay, ref by);
			if (blackMageIndex == 0) blackMageIndex = 1; 
			else if (blackMageIndex == 1) blackMageIndex = 0;
		}
		if (cx < bx || cx == bx && cy <= by) {
			swap(ref bx, ref cx);
			swap(ref by, ref cy);
			if (blackMageIndex == 1) blackMageIndex = 2;
			else if (blackMageIndex == 2) blackMageIndex = 1;
		}
		if (bx < ax || bx == ax && by <= ay) {
			swap(ref ax, ref bx);
			swap(ref ay, ref by);
			if (blackMageIndex == 0) blackMageIndex = 1;
			else if (blackMageIndex == 1) blackMageIndex = 0;
		}
		int index = heroX | (heroY << 2) | (ax << 4) | (ay << 7) | (bx << 10) | (by << 13) | (cx << 16) | (cy << 19) | (blackMageIndex << 22);
		//if (debug) {
		//    Debug.LogFormat("heroX = {0}, heroY = {1}, ax = {2}, ay = {3}, bx = {4}, by = {5}, cx = {6}, cy = {7}, blackMageIndex = {8}", heroX, heroY, ax, ay, bx, by, cx, cy, blackMageIndex);
		//    Debug.LogFormat("index = {0}", index);
		//}
		//if (debug) {
		//    Debug.LogFormat("a[index] = {0:0.####}", a[index]);
		//}
		result += a[index];
		result += (ax+ay+bx+by+cx+cy+dx+dy+blackMageIndex+decision) % 4 * randomCost;
		return result;
	}

	[ContextMenu("Multistep")]
	public void MultiStep() {
		StartCoroutine(SyncedMultiStep());
	}

	IEnumerator SyncedMultiStep() {
		for (int i = 0; i < stepCount; i++) {
			yield return StartCoroutine(Step());
			yield return new WaitForEndOfFrame();
		}
	}

	[ContextMenu("Step")]
	public void DoStep() {
		StartCoroutine(Step());
	}

	public IEnumerator Step() {
		double[] b = ClearMatrix();
		for (int i = 0; i < (1 << 24); i++) {
			b[i] = double.NaN;
		}
		for (int heroX = 0; heroX < N2; heroX++) {
			for (int heroY = heroX; heroY < N2; heroY++) {
				for (int ax = 0; ax < N; ax++) {
					for (int ay = 0; ay < N; ay++) {
						if (ax == heroX && ay == heroY) {
							continue;
						}
						Debug.LogFormat("step = {4}, heroX = {0}, heroY = {1}, ax = {2}, ay = {3}", heroX, heroY, ax, ay, step);
						yield return new WaitForEndOfFrame();
						for (int bx = 0; bx < N; bx++) {
							for (int by = 0; by < N; by++) {
								if (bx < ax || bx == ax && by <= ay) {
									continue;
								}
								if (bx == heroX && by == heroY) {
									continue;
								}
								for (int cx = 0; cx < N; cx++) {
									for (int cy = 0; cy < N; cy++) {
										if (cx < bx || cx == bx && cy <= by) {
											continue;
										}
										if (cx == heroX && cy == heroY) {
											continue;
										}
										for (int blackMageIndex = 0; blackMageIndex < 4; blackMageIndex++) {
											double result = 0;
											double cnt = 0;
											for (int dx = 0; dx < N; dx++) {
												for (int dy = 0; dy < N; dy++) {
													if (dx == heroX && dy == heroY || dx == ax && dy == ay || dx == bx && dy == by || dx == cx && dy == cy) {
														continue;
													}
													double best = double.NegativeInfinity;
													int bestDecision = -1;
													for (int decision = 0; decision < 4; decision++) {
														double cand = GetResult(heroX, heroY, ax, ay, bx, by, cx, cy, dx, dy, blackMageIndex, decision);
														if (cand > best) {
															best = cand;
															bestDecision = decision;
														}
													}
													result += best;
													++cnt;
												}
											}
											int index = heroX | (heroY << 2) | (ax << 4) | (ay << 7) | (bx << 10) | (by << 13) | (cx << 16) | (cy << 19) | (blackMageIndex << 22);
											b[index] = result / cnt;
										}
									}
								}
							}
						}
					}
				}
			}
		}
		a = b;
		++step;
		Debug.LogFormat("Step {0} complete", step);
		Save();
	}

	[ContextMenu("PrintResult")]
	public void PrintResult() {
		int heroX = Hero.instance.Position.x;
		int heroY = Hero.instance.Position.y;
		var bonuses = FindObjectsOfType<Heart>().Select(h => new Bonus(h.Position.x, h.Position.y, false, h.Position)).ToList();
		var bmp = BlackMage.instance.Position;
		bonuses.Add(new Bonus(bmp.x, bmp.y, true, bmp));
		var blackMageIndex = 3;
		int ax = bonuses[0].x;
		int ay = bonuses[0].y;
		int bx = bonuses[1].x;
		int by = bonuses[1].y;
		int cx = bonuses[2].x;
		int cy = bonuses[2].y;
		int dx = bonuses[3].x;
		int dy = bonuses[3].y;

		double best = double.NegativeInfinity;
		int bestDecision = -1;
		for (int decision = 0; decision < 4; decision++) {
			var targetCand = bonuses[decision].cell;
			double cand = GetResult(heroX, heroY, ax, ay, bx, by, cx, cy, dx, dy, blackMageIndex, decision, debug: logging);
			if (logging) {
				Debug.LogFormat("Candidate: {0:0.####}, goto ({1}, {2})", cand, targetCand.x, targetCand.y);
			}
			if (cand > best) {
				best = cand;
				bestDecision = decision;
			}
		}
		target = bonuses[bestDecision].cell;
		if (logging) {
			Debug.LogFormat("Result: {0:0.####}, goto ({1}, {2})", best, target.x, target.y);
		}
	}

	public void Move() {
		if (Hero.instance.health < 1 || BlackMage.instance.health < 1) {
			Controls.instance.Restart();
			return;
		}
		if (dynamicBlackMageCost) {
			blackMageCost = 1.0 * Hero.instance.health / BlackMage.instance.health;
		}
		PrintResult();
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
		if (Cheats.on) {
			if (Input.GetKeyDown(KeyCode.V)) {
				PrintResult();
			}
			if (Input.GetKeyDown(KeyCode.M)) {
				Move();
			}
		}
		if (on) {
			Move();
		}
	}
}
