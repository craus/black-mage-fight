﻿using UnityEngine;
using System.Collections;
using RSG;
using System;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.UI;

public class Controls : MonoBehaviour {
    public static Controls instance;
    const int N = 9;

    public Cell hovered;
    public Cell selected;

    public Unit activeUnit;

    public Button up;
    public Button down;
    public Button left;
    public Button right;

	public Vector3 anchorPosition;
	public float mouseStep = 20;
	public float singleMouseStep = 10;
	public Vector2 delta;
	public int maxMouseCharges = 1;
	public int mouseCharges = 1;

	public List<MonoBehaviour> lockers;

	public event Action onUnlocked = () => { };

	public UnityEvent ready;

	public List<ArrowButton> buttons;
	public bool timedMoves;
	public float moveTime = 1f;
	public float autoMoveTime;
	public float maxMovesAccumulated = 10;
	public bool autoSetTimerForDifficulty = false;

	public Slider moveTimerSlider;

	public void Lock(MonoBehaviour locker) {
		lockers.Add(locker);
	}

	public void Unlock(MonoBehaviour locker) {
		lockers.Remove(locker);
		if (!Locked()) {
			onUnlocked.Invoke();
		}
	}

	public bool Locked() {
		return lockers.Count > 0;
	}

	public void Reset() {
		lockers.Clear();
		commands = Promise.Resolved();
		autoMoveTime = float.PositiveInfinity;
	}

    void Awake() {
        instance = this;
		Ready();
    }

    private void RefreshHovered() {
        hovered = null;
        var cursor = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(new Ray(cursor, Vector3.forward), out hit)) {
            var cell = hit.collider.gameObject.GetComponent<Cell>();
            if (cell != null) {
                hovered = cell;
            }
        }
    }

    IPromise commands = Promise.Resolved();
	Promise after;

	void OnReady() {
		ready.Invoke();
	}

	public void Ready() {
		Command(Promise.Resolved);
	}

	void UpdateAfter(Promise newAfter) {
		if (after != null && after.CurState == PromiseState.Pending) {
			after.Reject(new RejectableException("Interrupted by new command"));
		}
		after = newAfter;
		after.Then(() => OnReady()).Done();
	}

    public void Command(Func<IPromise> command) {
		var ready = new Promise();
		UpdateAfter(ready);
		commands = commands.Then(command).Then(() => {
			if (ready.CurState == PromiseState.Pending) {
				ready.Resolve();
			}
		});
        commands.Done();
    }

    public void Move(IntVector2 direction) {
        if (Hero.instance.Dead || BlackMage.instance.Dead) {
            return;
        }
        if (Intermission.active) {
            return;
        }
        if (activeUnit == null) {
            return;
        }
		if (Locked()) {
			Debug.LogFormat("Locked");
			return;
		}
		autoMoveTime = Mathf.Clamp(autoMoveTime + moveTime, float.NegativeInfinity, TimeManager.Time() + maxMovesAccumulated * moveTime);
        Command(() => activeUnit.MoveTo(direction).Untyped());
    }

    public void Restart() {
        GameManager.instance.Restart();
    }

	public void Start() {
		#if UNITY_EDITOR
		Cheats.on = true;
		#endif 

		
	}

	public void MouseStep() {
		if (delta.x > Math.Abs(delta.y)) {
			Move(new IntVector2(0, 1));
		} else if (delta.x < -Math.Abs(delta.y)) {
			Move(new IntVector2(0, -1));
		} else if (delta.y < -Math.Abs(delta.x)) {
			Move(new IntVector2(1, 0));
		} else if (delta.y > Math.Abs(delta.x)) {
			Move(new IntVector2(-1, 0));
		}
		anchorPosition += delta.normalized.withZ(0) * mouseStep;
	}

    void Update() {
        if (Input.GetKeyDown(KeyCode.R) && Input.GetKey(KeyCode.LeftShift)) {
            GameManager.instance.DropSaveFile();
        }
        if (Input.GetKeyDown(KeyCode.Space)) {
			GameManager.instance.Continue();
        }
        if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0) {
            Cursor.visible = true;
        }
        if (Input.GetKeyDown(KeyCode.F5)) {
            Cheats.on ^= true;
        }
        if (Cheats.on) {
            if (Input.GetKeyDown(KeyCode.W)) {
				if (Input.GetKey(KeyCode.A)) {
					BlackMage.instance.health = 1;
				} else {
					GameManager.instance.Win();
				}
            }
            if (Input.GetKeyDown(KeyCode.L)) {
                GameManager.instance.Lose();
			}
			if (Input.GetKeyDown(KeyCode.R)) {
				GameManager.instance.Restart();
			}
            if (Input.GetKeyDown(KeyCode.LeftBracket)) {
				GameManager.instance.PreviousLevel();
            }
            if (Input.GetKeyDown(KeyCode.RightBracket)) {
				GameManager.instance.NextLevel();
            }

			if (Input.GetKeyDown(KeyCode.T)) {
				if (Input.GetKey(KeyCode.LeftShift)) {
					autoSetTimerForDifficulty ^= true;
					Debug.LogFormat($"autoSetTimerForDifficulty = {timedMoves}");
				} else {
					timedMoves ^= true;
					autoMoveTime = float.PositiveInfinity;
					Debug.LogFormat($"timedMoves = {timedMoves}");
				}
			}
			if (Input.GetKeyDown(KeyCode.Alpha1)) {
				moveTime *= 1.1f;
				Debug.LogFormat($"moveTime = {moveTime}");
			}
			if (Input.GetKeyDown(KeyCode.Alpha2)) {
				moveTime /= 1.1f;
				Debug.LogFormat($"moveTime = {moveTime}");
			}

			if (autoSetTimerForDifficulty) {
				var d = GameManager.instance.gameState.CurrentRun.difficulty;
				if (d == 0) {
					timedMoves = true;
					moveTime = 0.17f;
				}
				if (d == 1) {
					timedMoves = true;
					moveTime = 0.3f;
				}
				if (d == 2) {
					timedMoves = true;
					moveTime = 0.5f;
				}
				if (d == 3) {
					timedMoves = true;
					moveTime = 0.7f;
				}
				if (d == 4) {
					timedMoves = false;
				}
				if (d == 5) {
					timedMoves = false;
				}
			}

			if (timedMoves) {
				if (TimeManager.Time() > autoMoveTime) {
					buttons.rnd().Press();
				}
				moveTimerSlider.gameObject.SetActive(true);
				moveTimerSlider.value = maxMovesAccumulated - (autoMoveTime - TimeManager.Time()) / moveTime;
				moveTimerSlider.maxValue = maxMovesAccumulated;
			} else {
				moveTimerSlider.gameObject.SetActive(false);
			}

			if (Input.GetKeyDown(KeyCode.Minus)) {
				GameManager.instance.DecreaseDifficulty();
            }
			if (Input.GetKeyDown(KeyCode.Equals)) {
				GameManager.instance.IncreaseDifficulty();
            }

			if (Input.GetKeyDown(KeyCode.S)) {
				GameManager.instance.gameState.CurrentProfile.skipIntros ^= true;
			}
        }


        if (Input.GetKeyDown(KeyCode.Escape)) {
            UI.instance.Escape();
        }

		if (Input.GetMouseButtonDown(0)) {
			anchorPosition = Input.mousePosition;
			mouseCharges = maxMouseCharges;
		}
		delta = Input.mousePosition - anchorPosition;
		if (Input.GetMouseButton(0)) {
			if (mouseCharges > 0 && delta.magnitude > mouseStep) {
				MouseStep();
				--mouseCharges;
			}
		}
		if (Input.GetMouseButtonUp(0)) {
			if (mouseCharges > 0 && delta.magnitude > singleMouseStep) {
				MouseStep();
				--mouseCharges;
			}
		}
    }
}
