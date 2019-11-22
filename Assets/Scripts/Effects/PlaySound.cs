using UnityEngine;
using System.Collections;

public class PlaySound : Effect
{
    public AudioSource target;
    public float last = float.PositiveInfinity;
	public float from = 0;
	public float to = float.PositiveInfinity;
	public float fromPercent = 0;
	public float toPercent = 1;

    public override void Run() {
		var fromTime = Mathf.Max(fromPercent * target.clip.length, from, target.clip.length - last);
		target.time = fromTime;
        target.Play();
    }
}
