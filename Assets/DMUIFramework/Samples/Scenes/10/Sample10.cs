// [概要]
// UIレイヤーは再度タッチ状態になるとOnRetouchable()が呼び出されます。
// UI生成時は呼び出されません。
// [操作]
// 1. 緑画面のボタンを押す
// 2. 黄色四角内のボタンを押す
// [結果]
// 緑画面でボタンを押すと黄色の四角が表示されます。
// その後黄色四角内のボタンを押すと緑画面に戻りますが、
// この際"All Rigth"ログが表示されます。

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace DM {

	public class Sample10 : MonoBehaviour {

		void Start () {
			UIController.instance.Implement(new PrefabLoader(), new Sounder(), new FadeCreator());
			UIController.instance.AddFront(new Sample10Scene());
		}
	}

	class Sample10Scene : UIBase {

		public Sample10Scene() : base("UISceneA", UIGroup.Scene) {
		}

		public override IEnumerator OnLoaded() {
			root.Find("Layer/ButtonTop"   ).gameObject.SetActive(false);
			root.Find("Layer/ButtonBottom").gameObject.SetActive(false);

			yield break;
		}

		public override bool OnClick(string name, GameObject gameObject, PointerEventData pointer, SE se) {
			switch (name) {
				case "ButtonCenter": {
					UIController.instance.AddFront(new Sample10Dialog());
					return true;
				}
				default: {
					return false;
				}
			}
		}

		public override void OnRetouchable() {
			Debug.Log("Scene10 : All Right");
		}
	}

	class Sample10Dialog : UIBase {

		public Sample10Dialog() : base("UIDialog", UIGroup.Dialog, UIPreset.BackVisible) {
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
	}
}
