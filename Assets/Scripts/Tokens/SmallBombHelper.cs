using UnityEngine;
using System.Collections;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine.Events;
using System.Collections.Generic;

public class SmallBombHelper : MonoBehaviour
{
	public SpriteRenderer sprite;
	public SpriteRenderer dangerSprite;

	public Counter counter;
	public Explosive explosive;

	public void Awake() {
		counter = GetComponentInParent<Counter>();
		explosive = GetComponentInParent<Explosive>();
	}

	public void Start() {
		UpdateSprites();
	}

	public void UpdateSprites() {
		var danger = (counter.value == 1);
		Debug.LogFormat("{1} {0}", counter.transform.Path(), counter.value);
		sprite.gameObject.SetActive(!danger);
		dangerSprite.gameObject.SetActive(danger);

		if (danger) {
			explosive.ExplodeOnCells(false, cell => {
				cell.ChangeColor(Color.yellow);
			});
		}
	}
}
