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
	public class UIFade : UIBase {
		public UIFade(string path) : base(path, UIGroup.SystemFade, UIPreset.SystemIndicator) {
		}
	}
}