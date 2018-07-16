using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DM;

public class PrefabLoader : IPrefabLoader {
	public IEnumerator Load(string path, PrefabReceiver receiver) {
		ResourceRequest req = Resources.LoadAsync(path);
		yield return req;

		receiver.prefab = req.asset;
	}

	public void Release(string path, UnityEngine.Object prefab) {

	}
}


public class Sounder : ISounder {
	public void PlayDefaultClickSE() {
		Debug.Log("Sounder: DefaltClickSE");
	}
	public void PlayClickSE(string name) {
		Debug.Log("Sounder: ClickSE[" + name + "]");
	}
	public void PlayBGM(string name) {
		Debug.Log("Sounder: PlayBGM[" + name + "]");
	}
	public void StopBGM() {
		Debug.Log("Sounder: StopBGM");
	}
}


public class FadeCreator : IFadeCreator {
	public UIFade Create() {
		return new UIFade("BlackCurtainFade");
	}
}
