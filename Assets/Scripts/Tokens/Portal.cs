using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEngine.Events;

public class Portal : Figure
{
	public int id;
	Portal other;

	public UnityEvent onTeleport;

	void Start() {
		other = FindObjectsOfType<Portal>().First(p => p.id == this.id && p != this);
	}

	public override void Collide(Figure other) {
		var target = this.other.Position;
		Blink();
		this.other.Blink();
		target.MoveHere(other);
		onTeleport.Invoke();
	}
}
