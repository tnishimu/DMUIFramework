// [概要]
// UIPreset.TouchEventCallable使用した最前面に透明の画面全体のタッチ領域をAddFront()します。
// 画面を押すとタッチの反応としてログが流れます。ボタンを押すと緑の画面は削除されます。
// また、背後のボタンが反応するので、UIPreset.BackTouchableの利用例となります。
// [操作]
// 適宜画面をタッチ操作する
// [結果]
// 操作挙動に応じたログが表示されます。
// ボタンを押すと画面に何も表示されなくなります。

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace DM {

	public class Sample05 : MonoBehaviour {

		void Start () {
			UIController.instance.Implement(new PrefabLoader(), new Sounder(), new FadeCreator());
			UIController.instance.AddFront(new Sample05Scene());
		}
	}

	class Sample05Scene : UIBase {

		public Sample05Scene() : base("UISceneA", UIGroup.Scene) {
			UIController.instance.AddFront(new UISample05TouchLayer());
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
			Debug.Log("Scene05 : All Right");
		}
	}

	class UISample05TouchLayer : UIBase {

		public UISample05TouchLayer()
		: base("", UIGroup.System, UIPreset.BackVisible | UIPreset.BackTouchable | UIPreset.TouchEventCallable) {

		}

		public override bool OnTouchDown(string name, GameObject gameObject, PointerEventData pointer) {
			Debug.Log("touch down " + name + ": " + pointer.position);
			return false;
		}

		public override bool OnTouchUp(string name, GameObject gameObject, PointerEventData pointer) {
			Debug.Log("touch up " + name + ": " + pointer.position);
			return false;
		}

		public override bool OnDrag(string name, GameObject gameObject, PointerEventData pointer) {
			Debug.Log("touch drag " + name + ": " + pointer.position);
			return false;
		}
	}
}
