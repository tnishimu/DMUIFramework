// [概要]
// UIレイヤーが画面に表示した後にUIPartを追加したい場合は
// UIControler.AttachParts() で追加することができます。
// [操作]
// 1. 上部のボタンを押す
// 2. 増えたボタンを押す
// [結果]
// 上部のボタンを押すことで、下部にボタンが表示されます。
// 下部のボタンは押すとログを表示します。

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace DM {

	public class Sample15 : MonoBehaviour {

		void Start () {
			UIController.instance.Implement(new PrefabLoader(), new Sounder(), new FadeCreator());
			UIController.instance.AddFront(new Sample15Scene());
		}
	}

	class Sample15Scene : UIBase {

		private int m_count = 0;

		public Sample15Scene() : base("UISceneA", UIGroup.Scene) {
		}

		public override IEnumerator OnLoaded() {
			root.Find("Layer/ButtonCenter").gameObject.SetActive(false);
			root.Find("Layer/ButtonBottom").gameObject.SetActive(false);

			yield break;
		}

		public override bool OnClick(string name, GameObject gameObject, PointerEventData pointer, SE se) {
			switch (name) {
				case "ButtonTop": {
					m_count++;
					UIController.instance.AttachParts(this, new List<UIPart>(){new Sample15Button(m_count)});
					return true;
				}
				default: {
					return false;
				}
			}
		}
	}

	class Sample15Button : UIPart {

		private int m_id = 0;

		public Sample15Button(int id) : base("UIButton") {
			m_id = id;
		}

		public override IEnumerator OnLoaded(UIBase uiBase) {
			Text text = root.Find("Text").GetComponent<Text>();
			text.text = m_id.ToString();

			Transform layer = uiBase.root.Find("Layer");
			root.SetParent(layer);
			root.localPosition = new Vector3(426, 100 * m_id, 0);
			root.localScale = Vector3.one;

			yield break;
		}

		public override bool OnClick(string name, GameObject gameObject, PointerEventData pointer, SE se) {
			Debug.Log("push button: " + m_id);
			Debug.Log("Scene15 : All Right");

			return true;
		}
	}
}
