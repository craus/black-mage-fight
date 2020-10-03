using UnityEngine;
using System.Collections;
using RSG;

public class PlayerUnit : Unit
{
    public AudioSource moveSound;

    SpriteRenderer spriteRenderer;

	public override void Awake() {
		base.Awake();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    public override IPromise<bool> MoveTo(IntVector2 direction) {
		Debug.LogFormat("PlayerUnit.MoveTo {0} from {1}", direction, Position);  
        var oldPosition = Position;

		GameManager.instance.BeforeHeroMove();

        return base.MoveTo(direction).Then(moved => {
            if (moved) {
                moveSound.Play();
				Debug.LogFormat("moveSound.Play()");  
                CheckCollisions(oldPosition);
                GameManager.instance.HeroMoved(this, oldPosition, Position, direction);
                return;
            }
        });
    }

    public void CheckCollisions(Cell oldPosition) {
		return;
        foreach (Edge e in FindObjectsOfType<Edge>()) {
            if (e.position.a == oldPosition && e.position.b == Position) {
                e.Pick(this);
            }
            if (e.position.a == Position && e.position.b == oldPosition) {
                e.ReversePick(this);
            }
        }
    }

    public bool IsActiveUnit() {
        return Controls.instance.activeUnit == this;
    }

    void Update() {
        //spriteRenderer.material.ChangeAlpha(IsActiveUnit() ? 1 : 0.3f);
    }
}
