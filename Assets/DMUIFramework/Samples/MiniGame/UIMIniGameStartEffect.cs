using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DM;

public class UIMiniGameStartEffect : UIBase {

	public UIMiniGameStartEffect() : base("MiniGame/MiniGameStartEffect", UIGroup.Dialog, UIPreset.BackVisible) {
	}

	public override void OnActive() {
		UIController.instance.Dispatch("start", null);
		UIController.instance.Remove(this);
	}
}
