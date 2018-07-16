// [概要]
// 3Dオブジェクトを表示します。
// また、Terrainコンポーネントに対して、AddVisibleBehaviourController()を
// 使用することで表示の切り替えが行えます。
// [操作]
// 1. 黄四角内のボタンを押す
// 2. 再度黄四角内のボタンを押す
// 3. 球、キューブをタッチする
// [結果]
// 黄四角内のボタンを押すと3Dオブジェクトを表示します。
// さらに黄四角内のボタンを押すと黄四角が消えます。
// 球、キューブを押すと、画面外にはけて消えます。

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace DM {

	public class Sample20 : MonoBehaviour {

		void Start () {
			UIController.instance.Implement(new PrefabLoader(), new Sounder(), new FadeCreator());
			UIController.instance.AddFront(new Sample20Scene());
		}
	}

	class Sample20Scene : UIBase {

		public Sample20Scene() : base("UI3D", UIGroup.View3D, UIPreset.View3D) {
			AddVisibleBehaviourController<Terrain>();

			UIController.instance.AddFront(new Sample20Dialog(true));
			UIController.instance.AddFront(new Sample20Dialog(false));
		}

		public override bool OnClick(string name, GameObject gameObject, PointerEventData pointer, SE se) {
			if (name == "Cube" || name == "Sphere") {
				UIController.instance.Remove(this);
				Debug.Log("Scene20 : All Right");
				return true;
			}
			return false;
		}
	}

	class Sample20Dialog : UIBase {

		public Sample20Dialog(bool visible)
		: base("UIDialog", UIGroup.Dialog, visible ? UIPreset.BackVisible | UIPreset.BackTouchable : UIPreset.None) {
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
