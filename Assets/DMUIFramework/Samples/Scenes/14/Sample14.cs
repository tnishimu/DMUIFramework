// [概要]
// UIPartをUIBaseへ紐づける(YieldAttachParts())ことにより、
// UIPartとして個別に処理を分けることができます。
// 状況によって扱う数が変動するものをUIPartとして扱うと効果的です。
// 　例：リスト内の1項目を1つのUIPartとして扱う
// [操作]
// それぞれのボタンを押す
// [結果]
// 押したボタンに応じたログを表示します。

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace DM {

	public class Sample14 : MonoBehaviour {

		void Start () {
			UIController.instance.Implement(new PrefabLoader(), new Sounder(), new FadeCreator());
			UIController.instance.AddFront(new Sample14Scene());
		}
	}

	class Sample14Scene : UIBase {

		public Sample14Scene() : base("UISceneA", UIGroup.Scene) {
		}

		public override IEnumerator OnLoaded() {
			root.Find("Layer/ButtonTop"   ).gameObject.SetActive(false);
			root.Find("Layer/ButtonCenter").gameObject.SetActive(false);
			root.Find("Layer/ButtonBottom").gameObject.SetActive(false);

			List<UIPart> parts = new List<UIPart>();
			const int num = 4;
			for (int i = 1; i <= num; i++) {
				parts.Add(new Sample14Button(i));
			}
			yield return UIController.instance.YieldAttachParts(this, parts);
		}
	}

	class Sample14Button : UIPart {

		private int m_id = 0;

		public Sample14Button(int id) : base("UIButton") {
			m_id = id;
		}

		public override IEnumerator OnLoaded(UIBase uiBase) {
			Text text = root.Find("Text").GetComponent<Text>();
			text.text = m_id.ToString();

			// UIPartの追加先を決定する
			Transform layer = uiBase.root.Find("Layer");
			root.SetParent(layer);
			root.localPosition = new Vector3(426, 100 * m_id, 0);
			root.localScale = Vector3.one;

			yield break;
		}

		public override bool OnClick(string name, GameObject gameObject, PointerEventData pointer, SE se) {
			Debug.Log("push button: " + m_id);
			Debug.Log("Scene14 : All Right");

			return true;
		}
	}
}