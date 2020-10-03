using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Linq;
using RSG;

public class Letter : MonoBehaviour
{
	public Figure figure;
	public Marker marker;

	public LetterChecker checker;

	public UnityEvent afterSuccess;

	public GameObject activeImage;

	public Promise success;

	void Awake() {
		figure = GetComponent<Figure>();
		figure.afterMove.AddListener(AfterMove);
		marker = GetComponent<Marker>();
	}

	void AfterMove() {
		Controls.instance.Lock(this);
		checker.Change().Then(() => Controls.instance.Unlock(this)).Done();
	}

	public IPromise Success() {
		if (success == null) {
			success = new Promise();
			TimeManager
				.Wait(0)
				.Then(() => activeImage.SetActive(true))
				.Then(() => TimeManager.Wait(0.1f))
				.Then(() => activeImage?.SetActive(false))
				.Then(() => afterSuccess.Invoke())
				.Then(() => {
					success.Resolve();
					success = null;
				})
				.Done();
		}
		return success;
	}
}
