// [概要]
// UIController.SetScreenTouchable()を使用することによって
// 画面全体のタッチ反応を切ることができます。
// フェイルセーフとして呼び出したUIBaseが削除された場合、
// タッチ判定は元に戻ります。
// [操作]
// 画面下のボタンを押す
// [結果]
// ボタンを押すと数字が表示されますが、
// 100までの間は再度ボタンを押すことができません。

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace DM {

	public class Sample18 : MonoBehaviour {

		void Start () {
			UIController.instance.Implement(new PrefabLoader(), new Sounder(), new FadeCreator());
			UIController.instance.AddFront(new Sample18Scene());
		}
	}

	class Sample18Scene : UIBase {
		private int m_count = 0;

		public Sample18Scene() : base("UISceneA", UIGroup.Scene) {
		}

		public override IEnumerator OnLoaded() {
			root.Find("Layer/ButtonTop"   ).gameObject.SetActive(false);
			root.Find("Layer/ButtonCenter").gameObject.SetActive(false);

			yield break;
		}

		public override bool OnClick(string name, GameObject gameObject, PointerEventData pointer, SE se) {
			switch (name) {
				case "ButtonBottom": {
					UIController.instance.SetScreenTouchable(this, false);
					scheduleUpdate = true;
					m_count = 0;
					return true;
				}
				default: {
					return false;
				}
			}
		}

		public override void OnUpdate() {
			if (++m_count >= 100) {
				UIController.instance.SetScreenTouchable(this, true);
				scheduleUpdate = false;
			}
			Text text = root.Find("Layer/Text").GetComponent<Text>();
			text.text = m_count.ToString();
		}
	}
}
