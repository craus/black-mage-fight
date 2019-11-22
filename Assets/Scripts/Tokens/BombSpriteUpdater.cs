using UnityEngine;
using System.Collections;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine.Events;
using System.Collections.Generic;

public class BombSpriteUpdater : MonoBehaviour
{
	public SpriteRenderer sprite;
	public SpriteRenderer dangerSprite;

	public Counter counter;

	public void Awake() {
		counter = GetComponentInParent<Counter>();
		counter.onDecrement.AddListener(UpdateSprites);
		UpdateSprites();
	}

	public void UpdateSprites() {
		var danger = (counter.value == 1);
		sprite.gameObject.SetActive(!danger);
		dangerSprite.gameObject.SetActive(danger);
	}
}
