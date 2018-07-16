// [概要]
// バックキー挙動はUIBackableに指定してあるUIGroupのうち
// (デフォルトではScene, Dialog)
// もっとも手前にあるUIレイヤーを削除します。
// 削除時はOnBack()の呼び出しがあるので、削除自体を回避し、
// 別の挙動に変更することができます。
// [操作]
// 画面左下のボタンを押す
// [結果]
// ボタンを押すと黄四角が削除されます。
// さらにボタンを押すと青画面が削除されます。
// さらにボタンを押すと"All Right"ログが表示されます。

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace DM {

	public class Sample12 : MonoBehaviour {

		void Start () {
			UIController.instance.Implement(new PrefabLoader(), new Sounder(), new FadeCreator());
			UIController.instance.AddFront(new Sample12Scene());
		}
	}

	class Sample12Scene : UIBase {

		public Sample12Scene() : base("UISceneA", UIGroup.Scene) {
		}

		public override IEnumerator OnLoaded() {
			root.Find("Layer/ButtonTop"   ).gameObject.SetActive(false);
			root.Find("Layer/ButtonCenter").gameObject.SetActive(false);
			root.Find("Layer/ButtonBottom").gameObject.SetActive(false);

			UIController.instance.AddFront(new Sample12SceneB());
			UIController.instance.AddFront(new Sample12Frame());
			UIController.instance.AddFront(new Sample12Dialog());
			UIController.instance.AddFront(new Sample12Back());

			yield break;
		}

		public override bool OnBack() {
			Debug.Log("Scene12 : All Right");

			return false;
		}
	}

	class Sample12SceneB : UIBase {

		public Sample12SceneB() : base("UISceneB", UIGroup.Scene) {
		}

		public override IEnumerator OnLoaded() {
			root.Find("Layer/ButtonTop"   ).gameObject.SetActive(false);
			root.Find("Layer/ButtonCenter").gameObject.SetActive(false);
			root.Find("Layer/ButtonBottom").gameObject.SetActive(false);

			yield break;
		}
	}

	class Sample12Frame : UIBase {

		public Sample12Frame() : base("UIFrame", UIGroup.Floater, UIPreset.BackVisible) {
		}
	}

	class Sample12Dialog : UIBase {

		public Sample12Dialog() : base("UIDialog", UIGroup.Dialog, UIPreset.BackVisible) {
		}

		public override IEnumerator OnLoaded() {
			root.Find("Layer/ButtonCenter").gameObject.SetActive(false);

			yield break;
		}
	}

	class Sample12Back : UIBase {

		public Sample12Back() : base("UIBack", UIGroup.System, UIPreset.BackVisible) {
		}

		public override bool OnClick(string name, GameObject gameObject, PointerEventData pointer, SE se) {
			UIController.instance.Back();
			return true;
		}
	}
}
