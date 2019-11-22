using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ColorChanger : MonoBehaviour {
	public ValueProvider<Color> provider;

	[SerializeField]
	new Renderer renderer;

	public void Awake() {
		renderer = GetComponent<Renderer>();
		provider = GetComponent<ValueProvider<Color>>();
	}

	public void Start() {
		provider.onChange += ProviderOnChange;
		ProviderOnChange();
	}

	void ProviderOnChange() {
		renderer.material.color = provider.Value;
	}
}
