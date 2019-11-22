using UnityEngine;
using System.Collections;
using System.Linq;

public class Antidote : Figure
{
    public Poison poison;

    public override void Collide(Figure f) {
		base.Collide(f);
        if (f is Hero) {
            poison.Suppress();
            Relocate();
        }
    }
}
