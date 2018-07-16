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

	public class UIPartContainer {
		protected UnityEngine.Object m_prefab;
		public UnityEngine.Object prefab { get { return m_prefab; } }

		private UIPart m_ui;
		public UIPart ui { get { return m_ui; } }

		private UITouchListener[] m_listeners = null;

		public UIPartContainer(UIPart ui) {
			m_ui = ui;
		}

		public IEnumerator LoadAndSetup(UIBaseLayer layer) {
			if (m_ui.root == null && !string.IsNullOrEmpty(m_ui.prefabPath)) {
				PrefabReceiver receiver = new PrefabReceiver();
				yield return UIController.implements.prefabLoader.Load(m_ui.prefabPath, receiver);
				m_prefab = receiver.prefab;

				if (m_prefab != null) {
					GameObject g = GameObject.Instantiate(m_prefab) as GameObject;
					m_ui.root = g.transform;
				}
			}

			if (m_ui.root == null) {
				m_ui.root = new GameObject("root").transform;
			}
			m_ui.root.gameObject.SetActive(false);

			CollectComponents(m_ui.root.gameObject, layer);

			yield return m_ui.OnLoaded((UIBase)layer.m_ui);

			m_ui.root.gameObject.SetActive(true);
		}

		public virtual void Destroy() {
			UIController.implements.prefabLoader.Release(m_ui.prefabPath, m_prefab);
			m_prefab = null;

			m_ui.Destroy();
			m_ui = null;


			for (int i = 0; i < m_listeners.Length; i++) {
				m_listeners[i].ResetUI();
			}
			m_listeners = null;
		}

		protected void CollectComponents(GameObject target, UIBaseLayer layer) {
			m_listeners = target.GetComponentsInChildren<UITouchListener>();
			for (int i = 0; i < m_listeners.Length; i++) {
				m_listeners[i].SetUI(layer, m_ui);
			}

			Animator[] animators = target.GetComponentsInChildren<Animator>();
			m_ui.animators = animators;
		}
	}
}
