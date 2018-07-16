// ----------------------------------------------------------------------
// DMUIFramework
// Copyright (c) 2018 Takuya Nishimura (tnishimu)
//
// This software is released under the MIT License.
// https://opensource.org/licenses/mit-license.php
// ----------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DM {

	public enum UIGroup {
		None = 0,
		View3D,
		MainScene,
		Scene,
		Floater,
		Dialog,
		Debug,
		SystemFade,
		System,
	}

	class UIBackable {
		public static readonly List<UIGroup> groups = new List<UIGroup>(){
			UIGroup.Dialog,
			UIGroup.Scene,
			UIGroup.MainScene,
			UIGroup.View3D,
		};
		public static void Sort() {
			groups.Sort((x, y) => { return y - x; });
		}
	}

	class UIFadeTarget {
		public static readonly List<UIGroup> groups = new List<UIGroup>(){
			UIGroup.Floater,
			UIGroup.MainScene,
			UIGroup.View3D,
		};
	}

	class UIFadeThreshold {
		public static readonly Dictionary<UIGroup, int> groups = new Dictionary<UIGroup, int>(){
			{ UIGroup.Scene, 1 },
		};
	}
}