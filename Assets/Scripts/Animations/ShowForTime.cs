using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;

public class ShowForTime : MonoBehaviour
{
	public GameObject target;
	public float time;
	public UnityEvent afterStart;
	public UnityEvent then;
	public bool makeCopy;

	public void Run() {
		if (!makeCopy) {
			target.SetActive(true);
			afterStart.Invoke();
			TimeManager.Wait(time).Then(() => {
				target.SetActive(false);
				then.Invoke();
			});
		} else {
			var copy = Instantiate(target);
			copy.transform.SetParent(target.transform.parent, false);
			copy.transform.SetParent(null, true);
			copy.SetActive(true);			
			afterStart.Invoke();
			TimeManager.Wait(time).Then(() => {
				Destroy(copy);
				then.Invoke();
			});
		}
	}
}
