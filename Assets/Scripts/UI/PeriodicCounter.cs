using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PeriodicCounter : MonoBehaviour
{
    public Periodic periodic;
	public MultipleTimes multiple;

	public void Awake() {
		if (periodic == null) {
			periodic = GetComponent<Periodic>();
		}
		multiple = GetComponent<MultipleTimes>();
	}

	public bool Multiple() {
		return periodic.multipleRunPhases.Count > 1;
	}

    public int Value() {
        var phase = periodic.phase % periodic.Period;
		var value = Mathf.Min(
			periodic.runPhases.ExtMin(x => x > phase ? x - phase : x + periodic.Period - phase),
			periodic.multipleRunPhases.ExtMin(x => x.phase > phase ? x.phase - phase : x.phase + periodic.Period - phase)
		);
        return value;
    }

    public int MaxValue() {
		var phase = (periodic.phase + Value()) % periodic.Period;
		return Mathf.Min(
			periodic.runPhases.ExtMin(x => x > phase ? x - phase : x + periodic.Period - phase),
			periodic.multipleRunPhases.ExtMin(x => x.phase > phase ? x.phase - phase : x.phase + periodic.Period - phase)
		);
    }

	public int Delta() {
		if (multiple) {
			return multiple.Times;
		}
		return 1;
	}

	public string Format() {
		return string.Format(
			"<b>{0}/{1}{2}</b>",
			Value(),
			MaxValue(),
			Delta() > 1 ? string.Format("x{0}", Delta()) : ""
		);
	}

}
