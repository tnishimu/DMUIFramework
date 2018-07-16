// [概要]
// AddFront()によりUIレイヤーを3つ追加します。それぞれUIGroupは異なり、
// AddFront()による追加順番ではなく、UIGroupの階層順に表示されることを示します。
// また、背後は全て見えるため、UIPreset.BackVisibleの利用例ともなります。
// [操作]
// 操作なし
// [結果]
// 緑の画面が表示され、その手前上下に赤い帯が表示されます。
// さらにその手前に黄色の四角が表示されます。(3:4の画面サイズ時)

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace DM {

	public class Sample03 : MonoBehaviour {

		void Start () {
			UIController.instance.Implement(new PrefabLoader(), new Sounder(), new FadeCreator());
			UIController.instance.AddFront(new Sample03Scene());
		}
	}

	class Sample03Scene : UIBase {

		public Sample03Scene() : base("UISceneA", UIGroup.Scene) {
			UIController.instance.AddFront(new Sample03Frame());
		}

		public override IEnumerator OnLoaded() {
			root.Find("Layer/ButtonTop"   ).gameObject.SetActive(false);
			root.Find("Layer/ButtonCenter").gameObject.SetActive(false);
			root.Find("Layer/ButtonBottom").gameObject.SetActive(false);

			UIController.instance.AddFront(new Sample03Dialog());
			yield break;
		}
	}

	class Sample03Frame : UIBase {

		public Sample03Frame() : base("UIFrame", UIGroup.Floater, UIPreset.BackVisible) {
		}
	}

	class Sample03Dialog : UIBase {

		public Sample03Dialog() : base("UIDialog", UIGroup.Dialog, UIPreset.BackVisible) {
		}

		public override IEnumerator OnLoaded() {
			root.Find("Layer/ButtonCenter").gameObject.SetActive(false);

			Debug.Log("Scene03 : All Right");
			yield break;
		}
	}
}
