// [概要]
// 同UIレイヤー内に複数の登場アニメーションがある場合、
// もっとも最後に終わるアニメーションを持って、登場アニメーションの完了となります。
// また、登場アニメーション後に呼び出されるOnActive()のメソッドも使用しています。
// [操作]
// 1. 緑画面の登場アニメーション中にボタンを押す。（なにも反応しない）
// 2. 緑画面は止まった中、白い四角が点滅している間にボタンを押す。（なにも反応しない）
// 3. アニメーション終了後、ボタンを押す。
// [結果]
// 緑の画面が右から登場します。同時に白い四角が点滅します。
// 緑の画面が止まっても、まだ白い四角が点滅します。
// 白い四角の点滅が終わり次第、ボタンが押せるようになります。

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace DM {

	public class Sample08 : MonoBehaviour {

		void Start () {
			UIController.instance.Implement(new PrefabLoader(), new Sounder(), new FadeCreator());
			UIController.instance.AddFront(new Sample08Scene());
		}
	}

	class Sample08Scene : UIBase {

		public Sample08Scene() : base("UISceneAnimA", UIGroup.Scene) {
		}

		public override IEnumerator OnLoaded() {
			root.Find("Layer/ButtonTop"   ).gameObject.SetActive(false);
			root.Find("Layer/ButtonBottom").gameObject.SetActive(false);
			root.Find("Layer/Square").gameObject.SetActive(true);

			yield break;
		}

		public void onActive() {
			Debug.Log("in animation end");
		}

		public override bool OnClick(string name, GameObject gameObject, PointerEventData pointer, SE se) {
			switch (name) {
				case "ButtonCenter": {
					Debug.Log("start removing animation");
					UIController.instance.Remove(this);
					return true;
				}
				default: {
					return false;
				}
			}
		}

		public override void OnDestroy() {
			Debug.Log("Scene08 : All Right");
		}
	}
}
