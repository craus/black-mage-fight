using UnityEngine;
using System.Collections;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine.Events;
using System;
using RSG;
using System.Collections.Generic;

public class Figure : Token
{
    [SerializeField]
    private Cell position;

	public FigureEvent onCollide;
	public FigureEvent onUncollide;
	public IntVector2Event onPushed;

	public UnityEvent beforeMove;
	public UnityEvent afterMove;

    public AudioSource swapSound;

	public Blink blink;

    public Cell Position {
        get { return position; }
    }

    public virtual void BeforeLeaveCell() {
		beforeMove.Invoke();
    }

    public virtual void AfterEnterCell() {
		afterMove.Invoke();
    }

	public bool Marked(Mark mark) {
		return GetComponents<Marker>().Any(m => m.mark == mark);
	}

	public bool Marked(IEnumerable<Mark> marks) {
		return marks.Any(Marked);
	}

	public bool Marked(params Mark[] marks) {
		return marks.Any(Marked);
	}

    public void SetPosition(Cell cell) {
        if (position != null) {
            BeforeLeaveCell();
            position.figures.Remove(this);
        }
        position = cell;
        if (position != null) {
            position.figures.Add(this);
            AfterEnterCell();
        }
    }

	public virtual void Awake() {
		blink = GetComponent<Blink>();
		//Tracker
	}

    bool Swap(IntVector2 direction) {
        var cell = Position.ToDirection(direction);
		for (int i = 0; i < 100 && cell != null && !cell.figures.Any(f => f is Swapper); i++) {
            cell = cell.ToDirection(direction);
        }
        if (cell != null) {
            var s = cell.figures.FirstOrDefault(f => f is Swapper);
			if (s != null) {
				var x = Position;
				cell.MoveHere(this);
				x.MoveHere(s);
				if (swapSound != null) {
					swapSound.Play();
				}
				return true;
			}
        }
        return false;
    }

    public virtual void OnSee(Figure other) {
    }

    public void See(IntVector2 direction) {
        var cell = Position.ToDirection(direction);
		for (int i = 0; i < 100 && cell != null && !cell.figures.Any(f => f is EvilEye); i++) {
            cell = cell.ToDirection(direction);
        }
        if (cell != null) {
			//cell.figures.First().GetComponent<OnSee>().Run();
			var figure = cell.figures.FirstOrDefault();
			if (figure != null) {
				figure.OnSee(this);
			}
        }
    }

    public virtual IPromise<bool> MoveTo(IntVector2 direction) {
		if (EventManager.instance.BeforeFigureMove(this, direction).cancelEvent) {
			return Promise.v(false);
		}

        See(direction);
        if (Swap(direction)) {
            return Promise<bool>.Resolved(true);
        }
        var cell = Position.ToDirection(direction);
		for (int i = 0; i < 100 && cell != null && cell.figures.Any(f => f is Ice); i++) {
            cell = cell.ToDirection(direction);
        }
        if (cell == null) {
            return Promise.v(false);
        }
		cell.figures.ForEach(f => f.onPushed.Invoke(direction));
        cell.MoveHere(this);
        return Promise.v(true);
    }

    [ContextMenu("Place")]
    public void Place() {
        position.MoveHere(this);
#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
#endif
    }

	[ContextMenu("Blink")]
	public void Blink() {
		Blink(null);
	}

    public void Blink(Func<Figure, bool> occupies) {
        Board.instance.RandomEmptyCell(occupies).MoveHere(this);
    }

    public void Relocate() {
        if (blink != null) {
            blink.Run();
            return;
        }
//        var locators = FindObjectsOfType<Locator>().ToList();
//        if (locators.Count > 0) {
//            locators.Rnd().LocateHere(this);
//            return;
//        }
        var trajectoryPosition = Position.figures.FirstOrDefault(f => f is TrajectoryPosition) as TrajectoryPosition;
        if (trajectoryPosition != null) {
			trajectoryPosition.trajectory.positions.CyclicNext(trajectoryPosition).position.MoveHere(this);
            return;
        }
        Blink();
    }

    public virtual void Collide(Figure other) {
        onCollide.Invoke(other);
    }

	public virtual void Uncollide(Figure other) {
		onUncollide.Invoke(other); 
	}

    public virtual bool Occupies() {
        return true;
    }

    protected virtual void OnDestroy() {
        //Debug.LogFormat("OnDestroy {0}", transform.Path());
        if (position != null) {
            //Debug.LogFormat("Remove {0} from {1}", transform.Path(), position.transform.Path());
            position.figures.Remove(this);
        }
    }
}
