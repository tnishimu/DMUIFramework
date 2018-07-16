using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DM;

public class UIMiniGameTitle : UIBase {

	public UIMiniGameTitle() : base("MiniGame/MiniGameTitle", UIGroup.MainScene) {
	}

	public override bool OnClick(string name, GameObject gameObject, PointerEventData pointer, SE se) {
		switch (name) {
			case "HowToPlay": {
				UIController.instance.AddFront(new UIMiniGameHowToPlay());
				return true;
			}
			case "Panel": {
				UIController.instance.Replace(new UIBase[]{ new UIMiniGameMain() });
				return true;
			}
		}
		return false;
	}
}
