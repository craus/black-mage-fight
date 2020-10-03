using UnityEngine;
using System.Collections;
using System.Linq;

public class Heart : Figure
{
    public static Heart instance;

    [SerializeField] private int heal = 3;
    public IntValueProvider healProvider;

    public int Heal => healProvider != null ? healProvider.Value : heal;

    public override void Collide(Figure f) {
        var hero = f as PlayerUnit;
        if (hero != null) {
            hero.Heal(Heal);
            Relocate();
        }
    }

    public override void Awake() {
        base.Awake();
        instance = this;
    }
}
