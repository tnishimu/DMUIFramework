// [概要]
// UIレイヤーはOnUpdate()の呼び出しを明示すると、
// UIController コンポーネント の Update()と同フレームで
// OnUpdate()が呼び出されます。
// また、UIレイヤーは他のレイヤー全てに向けてイベントを発信することができます。
// [操作]
// 画面下のボタンを押す
// [結果]
// 緑画面下のボタンを押すと
// 黄四角内に、更新回数が表示されます。

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace DM {

	public class Sample11 : MonoBehaviour {

		void Start () {
			UIController.instance.Implement(new PrefabLoader(), new Sounder(), new FadeCreator());
			UIController.instance.AddFront(new Sample11Scene());
		}
	}

	class DispachParams {
		public int count;
		public DispachParams(int c) {
			count = c;
		}
	}

	class Sample11Scene : UIBase {

		private int m_count = 0;

		public Sample11Scene() : base("UISceneA", UIGroup.Scene) {
		}

		public override IEnumerator OnLoaded() {
			root.Find("Layer/ButtonTop"   ).gameObject.SetActive(false);
			root.Find("Layer/ButtonCenter").gameObject.SetActive(false);

			UIController.instance.AddFront(new Sample11Dialog());

			scheduleUpdate = true;

			yield break;
		}

		public override bool OnClick(string name, GameObject gameObject, PointerEventData pointer, SE se) {
			switch (name) {
				case "ButtonBottom": {
					UIController.instance.Dispatch("Sample", new DispachParams(m_count));
					return true;
				}
				default: {
					return false;
				}
			}
		}

		public override void OnUpdate() {
			m_count++;
		}
	}


	class Sample11Dialog : UIBase {

		public Sample11Dialog() : base("UIDialog", UIGroup.Dialog, UIPreset.BackVisible | UIPreset.BackTouchable) {
		}

		public override IEnumerator OnLoaded() {
			root.Find("Layer/ButtonCenter").gameObject.SetActive(false);

			yield break;
		}

		public override void OnDispatchedEvent(string name, object param) {
			if (name == "Sample") {
				Text text = root.Find("Layer/Text").GetComponent<Text>();
				text.text = ((DispachParams)param).count.ToString();
				Debug.Log("Scene11 : All Right");
			}
		}
	}
}
