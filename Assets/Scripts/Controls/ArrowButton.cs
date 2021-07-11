using UnityEngine;
using System.Collections;
using System;

public class ArrowButton : Button {
    //min time for slow gamers 
    private const int DELAY_BETWEEN_PRESS_ACTIONS = 220;

    private static bool arrowPressed;
    private static int oldTick = Int32.MaxValue;

    public IntVector2 direction;
    private bool pressed;

    private bool IsNewTick(int tick) {
        if (tick < oldTick) {
            //handle int overflow after 49 days
            if (tick > (Int32.MinValue + DELAY_BETWEEN_PRESS_ACTIONS)) {
                oldTick = tick;
                return true;
            }
            return false;
        }
        if (tick - oldTick > DELAY_BETWEEN_PRESS_ACTIONS) {
            oldTick = tick;
            return true;
        }
        return false;
    }

    private void Awake() {
        arrowPressed = false;
        pressed = false;
    }

    void Update() {
        if (Input.GetButtonUp(button)) {
            if (pressed) {
                arrowPressed = false;
            }
            pressed = false;
        } else if (!arrowPressed || (arrowPressed && pressed)) {
            if (Input.GetButton(button) || Input.GetButtonDown(button)) {
                //Debug.Log("frame_count " + Time.frameCount);
                if (pressed) {
                    CheckForPress();
                } else {
                    //else branch is faster for manual clicks(with release button)
                    pressed = true;
                    arrowPressed = true;
                    oldTick = Environment.TickCount;
                    Press();
                }
            }
        }
    }

    private void CheckForPress() {
        if (IsNewTick(Environment.TickCount)) {
            Press();
        }
    }

    public override void Press() {
		if (UI.instance.menu.gameObject.activeSelf) {
			return;
		}
        Controls.instance.Move(direction);
        Cursor.visible = false;
    }
}
