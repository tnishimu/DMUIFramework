// [概要]
// UIレイヤーの存在チェックをいくつかの調べ方で確認できます。
// ・HasUI() : UIとして存在しているかチェックします
// ・GetFrontUINameInGroup() : 指定グループ内の最前面のUIの名前を取得します。
// ・GetUINumInGroup() : 指定グループ内にいくつUIが存在しているか取得します。
// [操作]
// 黄四角内のボタンを押す
// [結果]
// ログにそれぞれの調べた内容が表示されます。
//
// SceneA: True
// SceneB: True
// SceneC: False
// SceneFront: Sample19SceneB
// FloaterFront: Sample19Frame
// SystemFront:
// SceneNum: 2
// FloaterNum: 1
// SystemNum: 0

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace DM {

	public class Sample19 : MonoBehaviour {

		void Start () {
			UIController.instance.Implement(new PrefabLoader(), null, new FadeCreator());
			UIController.instance.AddFront(new Sample19Scene());
		}
	}
	class Sample19Scene : UIBase {
		public Sample19Scene() : base("UISceneA", UIGroup.Scene) {
		}

		public override IEnumerator OnLoaded() {
			root.Find("Layer/ButtonTop"   ).gameObject.SetActive(false);
			root.Find("Layer/ButtonCenter").gameObject.SetActive(false);
			root.Find("Layer/ButtonBottom").gameObject.SetActive(false);

			UIController.instance.AddFront(new Sample19SceneB());
			UIController.instance.AddFront(new Sample19Frame());
			UIController.instance.AddFront(new Sample19Dialog());

			yield break;
		}
	}

	class Sample19SceneB : UIBase {
		public Sample19SceneB() : base("UISceneB", UIGroup.Scene) {
		}

		public override IEnumerator OnLoaded() {
			root.Find("Layer/ButtonTop"   ).gameObject.SetActive(false);
			root.Find("Layer/ButtonCenter").gameObject.SetActive(false);
			root.Find("Layer/ButtonBottom").gameObject.SetActive(false);

			yield break;
		}
	}

	class Sample19Frame : UIBase {
		public Sample19Frame() : base("UIFrame", UIGroup.Floater, UIPreset.BackVisible) {
		}
	}

	class Sample19Dialog : UIBase {
		public Sample19Dialog() : base("UIDialog", UIGroup.Dialog, UIPreset.BackVisible) {
		}

		public override bool OnClick(string name, GameObject gameObject, PointerEventData pointer, SE se) {
			switch (name) {
				case "ButtonCenter": {
					Debug.Log("SceneA: " + UIController.instance.HasUI("Sample19Scene"));
					Debug.Log("SceneB: " + UIController.instance.HasUI("Sample19SceneB"));
					Debug.Log("SceneC: " + UIController.instance.HasUI("Sample19SceneC"));
					Debug.Log("SceneFront: "   + UIController.instance.GetFrontUINameInGroup(UIGroup.Scene));
					Debug.Log("FloaterFront: " + UIController.instance.GetFrontUINameInGroup(UIGroup.Floater));
					Debug.Log("SystemFront: "  + UIController.instance.GetFrontUINameInGroup(UIGroup.System));
					Debug.Log("SceneNum: "   + UIController.instance.GetUINumInGroup(UIGroup.Scene));
					Debug.Log("FloaterNum: " + UIController.instance.GetUINumInGroup(UIGroup.Floater));
					Debug.Log("SystemNum: "  + UIController.instance.GetUINumInGroup(UIGroup.System));
					return true;
				}
				default: {
					return false;
				}
			}
		}
	}
}
