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
        target.time = target.clip.length - last;
        target.Play();
    }
}
