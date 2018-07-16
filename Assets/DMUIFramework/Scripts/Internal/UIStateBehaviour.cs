// ----------------------------------------------------------------------
// DMUIFramework
// Copyright (c) 2018 Takuya Nishimura (tnishimu)
//
// This software is released under the MIT License.
// https://opensource.org/licenses/mit-license.php
// ----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DM {

	public class UIStateBehaviour : StateMachineBehaviour {

		public static readonly string LayerName = "UI.";

		private Action<Animator> m_exitCallback;
		public Action<Animator> ExitCallback { set { m_exitCallback = value; } }

		private string m_playName;
		public string PlayName { set { m_playName = value; } }

		override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
			AnimatorClipInfo[] infos = animator.GetCurrentAnimatorClipInfo(layerIndex);
			if (stateInfo.IsName(m_playName) && infos.Length == 0 && m_exitCallback != null) {
				m_exitCallback(animator);
				m_exitCallback = null;
			}
		}

		override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
			if (stateInfo.IsName(m_playName) && m_exitCallback != null) {
				m_exitCallback(animator);
				m_exitCallback = null;
			}
		}
	}
}