using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DM;

public class MiniGame : MonoBehaviour {

	void Start () {
		UIController.instance.Implement(new PrefabLoader(), null, new FadeCreator());
		UIController.instance.AddFront(new UIMiniGameTitle());
	}
}
