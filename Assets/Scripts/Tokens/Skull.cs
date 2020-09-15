using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.Events;

public class Skull : Figure
{
    public int damage;
	public int step;
    public TextMesh text;
	public SpriteRenderer sprite;
	public SpriteRenderer safeSprite;

	public UnityEvent onHit;

    void Start() {
        UpdateUI();
    }

    public void Increment() {
        damage += step; 
		step += 1;
		UpdateUI();
    }

    public void Spend(int value) {
        damage -= value;
        UpdateUI();
    }

    public void UpdateUI() {
		text.text = damage.ToString();
		var scale = 0.3f + 0.1f * Mathf.Clamp(step, 0, 8);
		transform.localScale = new Vector3(scale, scale, 1);
		if (safeSprite) {
			sprite.enabled = (damage > 0);
			safeSprite.enabled = (damage == 0);
		}
    }

    public override bool Occupies() {
        return false;
    }

    public override void Collide(Figure f) {
        var hero = f as Hero;
        if (hero != null) {
			if (damage > 0) {
				onHit.Invoke();
			}
            hero.Hit(damage);
        }
        if (f != this) {
            Destroy(gameObject);
        }
    }
}
