// [概要]
// UIImplementsにより音再生は外部からアタッチされますが、
// OnClick()メソッドでreturn trueを行うとデフォルトのクリック音を再生します。
// また、BGMはBGMが設定してあり表示されているレイヤーのうち、
// 最も手前のUIレイヤーのBGMを再生します。
// [操作]
// ボタンを押す
// [結果]
// ボタンを押すとデフォルトではないSE再生ログが表示されます。
// また、緑/青画面で設定したBGM再生のログが表示されます。

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace DM {

	public class Sample13 : MonoBehaviour {

		void Start () {
			UIController.instance.Implement(new PrefabLoader(), new Sounder(), new FadeCreator());
			UIController.instance.AddFront(new Sample13Scene());
		}
	}

	class Sample13Scene : UIBase {

		public Sample13Scene() : base("UISceneA", UIGroup.Scene, UIPreset.None, "BGM_SceneA") {
		}

		public override IEnumerator OnLoaded() {
			root.Find("Layer/ButtonTop"   ).gameObject.SetActive(false);
			root.Find("Layer/ButtonBottom").gameObject.SetActive(false);

			yield break;
		}

		public override bool OnClick(string name, GameObject gameObject, PointerEventData pointer, SE se) {
			switch (name) {
				case "ButtonCenter": {
					UIController.instance.AddFront(new Sample13SceneB());

					se.playName = "Center SE";
					return true;
				}
				default: {
					return false;
				}
			}
		}
	}


	class Sample13SceneB : UIBase {

		public Sample13SceneB() : base("UISceneAnimB", UIGroup.Scene, UIPreset.None, "BGM_SceneB") {
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
					Debug.Log("Scene13 : All Right");
					return true;
				}
				default: {
					return false;
				}
			}
		}
	}
}
