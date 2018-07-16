using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DM;

public class UIMiniGameHowToPlay : UIBase {

	public UIMiniGameHowToPlay() : base("MiniGame/MiniGameHowToPlay", UIGroup.Dialog, UIPreset.BackVisible | UIPreset.TouchEventCallable) {
	}

	public override bool OnTouchUp(string name, GameObject gameObject, PointerEventData pointer) {
		UIController.instance.Remove(this);
		return true;
	}
}
