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
using UnityEngine.EventSystems;

namespace DM {

	public class UIPart {
		public class SE {
			public string playName = "";
		}

		private Animator[] m_animators;
		public Animator[] animators { set { m_animators = value; } }

		private int m_playCount = 0;
		private Action m_stopCallback = null;

		private Transform m_root;
		public Transform root {
			get { return m_root; }
			set { m_root = value; }
		}
		readonly string m_prefabPath;
		public string prefabPath { get { return m_prefabPath; } }

		private bool m_exit;

		public UIPart(Transform root) {
			m_root = root;
		}
		public UIPart(string path) {
			m_prefabPath = path;
		}

		public virtual void Destroy() {
			OnDestroy();

			if (m_root != null) {
				m_root.SetParent(null);
				GameObject.Destroy(m_root.gameObject);
				m_root = null;
			}

			m_animators = null;
			m_stopCallback = null;
		}

		public bool PlayAnimations(string name, Action callback = null, bool exit = false) {
			if (m_playCount > 0) {
				return false;
			}

			m_exit = exit;

			int count = Play(name);
			if (count <= 0) {
				return false;
			}

			if (callback != null) {
				m_playCount    = count;
				m_stopCallback = callback;
			}

			return true;
		}

		private int Play(string name) {
			string playName = UIStateBehaviour.LayerName + name;

			int count = 0;
			for (int i = 0; i < m_animators.Length; i++) {
				UIStateBehaviour[] states = m_animators[i].GetBehaviours<UIStateBehaviour>();
				for (int j = 0; j < states.Length; j++) {
					states[j].ExitCallback = onExit;
					states[j].PlayName = playName;
				}
				if (states.Length > 0) {
					m_animators[i].Play(playName);
					++count;
				}
			}
			return count;
		}

		private void onExit(Animator animator) {
			if (m_exit) {
				animator.enabled = false;
			}

			if (--m_playCount <= 0) {
				if (m_stopCallback != null) {
					m_stopCallback();
				}
			}
		}

		// -----------------------------------------------------------------------------------------------------------------------------------------
		// virtual methods

		public virtual IEnumerator OnLoaded(UIBase uiBase) { yield break; }

		public virtual bool OnClick(string name, GameObject gameObject, PointerEventData pointer, SE se) { return false; }

		public virtual bool OnTouchDown(string name, GameObject gameObject, PointerEventData pointer) { return false; }

		public virtual bool OnTouchUp(string name, GameObject gameObject, PointerEventData pointer) { return false; }

		public virtual bool OnDrag(string name, GameObject gameObject, PointerEventData pointer) { return false; }

		public virtual void OnDestroy() { }
	}
}