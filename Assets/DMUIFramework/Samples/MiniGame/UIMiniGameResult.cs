using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DM;

public class UIMiniGameResult : UIBase {

	private float m_score;

	public UIMiniGameResult(float score) : base("MiniGame/MiniGameResult", UIGroup.Dialog, UIPreset.BackVisible) {
		m_score = score;
	}

	public override IEnumerator OnLoaded() {
		Text score = root.Find("Panel/Score").GetComponent<Text>();
		score.text = m_score.ToString("N2");

		yield break;
	}

	public override bool OnClick(string name, GameObject gameObject, PointerEventData pointer, SE se) {
		switch (name) {
			case "Title": {
				UIController.instance.Replace(new UIBase[] { new UIMiniGameTitle() }, new UIGroup[]{ UIGroup.Dialog });
				return true;
			}
			case "Retry": {
				UIController.instance.Dispatch("retry", null);
				UIController.instance.Remove(this);
				return true;
			}
		}
		return false;
	}
}
