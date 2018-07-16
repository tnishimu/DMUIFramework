// [概要]
// ボタンクリック時の挙動としてUIレイヤーの削除を行います。
// また、OnDestroy()による削除時に実行されるメソッドも使用しています。
// [操作]
// ボタンをクリックする
// [結果]
// 緑の画面中央にボタンが表示され、ボタンを押すと画面上に何も表示されません。

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace DM {

	public class Sample02 : MonoBehaviour {

		void Start () {
			UIController.instance.Implement(new PrefabLoader(), new Sounder(), new FadeCreator());
			UIController.instance.AddFront(new Sample02Scene());
		}
	}

	class Sample02Scene : UIBase {

		public Sample02Scene() : base("UISceneA", UIGroup.Scene) {
		}

		public override IEnumerator OnLoaded() {
			root.Find("Layer/ButtonTop"   ).gameObject.SetActive(false);
			root.Find("Layer/ButtonBottom").gameObject.SetActive(false);

			yield break;
		}

		public override bool OnClick(string name, GameObject gameObject, PointerEventData pointer, SE se) {
			switch (name) {
				case "ButtonCenter": {
					UIController.instance.Remove(this);
					return true;
				}
				default: {
					return false;
				}
			}
		}

		public override void OnDestroy() {
			Debug.Log("Scene02 : All Right");
		}
	}
}
