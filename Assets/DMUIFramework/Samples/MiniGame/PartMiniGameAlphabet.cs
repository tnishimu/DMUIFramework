using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DM;

public class PartMiniGameAlphabet : UIPart {

	private UIMiniGameMain m_main;
	private char m_alphabet;

	public PartMiniGameAlphabet(UIMiniGameMain main, char alphabet) : base("MiniGame/MiniGameAlphabet") {
		m_main = main;
		m_alphabet = alphabet;
	}

	public override IEnumerator OnLoaded(UIBase uiBase) {
		root.SetParent(uiBase.root.Find("Panel"));
		root.localScale = Vector3.one;

		Transform alphabet = root.Find("Button/Alphabet");
		Image img = alphabet.GetComponent<Image>();
		img.sprite = Resources.Load<Sprite>("MiniGame/Images/" + m_alphabet.ToString());

		root.Find("Button").gameObject.SetActive(false);

		yield break;
	}

	public override bool OnClick(string name, GameObject gameObject, PointerEventData pointer, SE se) {
		if (m_main.Check(m_alphabet)) {
			root.Find("Button").gameObject.SetActive(false);
		}
		return true;
	}

	public void SetPosition(Vector2 pos) {
		root.localPosition = pos;
	}

	public void Open() {
		root.Find("Button").gameObject.SetActive(true);
		root.GetComponent<Animation>().Play();
	}
}
