using UnityEngine;
using System.Collections;

public class ArrowButton : Button {
    public IntVector2 direction;

    public override void Press() {
		if (UI.instance.menu.gameObject.activeSelf) {
			return;
		}
        Controls.instance.Move(direction);
        Cursor.visible = false;
    }
}
