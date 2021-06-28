using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Linq;

public class SkullSetter : Token
{
	public static SkullSetter instance;

    public GameObject skullPrefab;

    public Periodic periodic;

    public void SetSkull() {
        var target = Board.instance.RandomEmptyCell();
		var skull = target.figures.FirstOrDefault(f => f is Skull) as Skull;
        if (skull != null) {
            skull.Increment();
        } else {
            skull = Instantiate(skullPrefab).GetComponent<Skull>();
            target.MoveHere(skull);
        }
    }

	public void Awake() {
		instance = this;
	}

    public void Update() {
        if (Cheats.on) {
            if (Input.GetKeyDown(KeyCode.V)) {
                SetSkull();
            }
        }
    }
}
