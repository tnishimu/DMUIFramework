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

	public class UIVisibleController {
		private Dictionary<Component, bool> m_components = new Dictionary<Component, bool>();

		public void SetVisible(GameObject target, bool enable) {
			if (m_components == null) { return; }

			if (enable) {
				if (IsVisible()) { return; }

				foreach (KeyValuePair<Component, bool> pair in m_components) {
					SetEnable(pair.Key, pair.Value);
				}
				m_components.Clear();

			} else {
				if (!IsVisible()) { return; }

				Component[] components = GetComponents(target);
				for (int i = 0; i < components.Length; i++) {
					Component component = components[i];
					m_components.Add(component, IsEnable(component));
					SetEnable(component, false);
				}
			}
		}

		public void Destroy() {
			m_components = null;
		}

		public bool IsVisible() {
			return (m_components.Count == 0);
		}

		public virtual Component[] GetComponents(GameObject target) { return null; }
		public virtual void SetEnable(Component component, bool enable) { }
		public virtual bool IsEnable(Component component) { return true; }
	}

	public class UIBehaviourController<T> : UIVisibleController where T : Behaviour {

		public override Component[] GetComponents(GameObject target) {
			return target.GetComponentsInChildren<T>();
		}
		public override void SetEnable(Component component, bool enable) {
			(component as T).enabled = enable;
		}
		public override bool IsEnable(Component component) {
			return (component as T).enabled;
		}
	}

	public class UIRendererController : UIVisibleController {

		public override Component[] GetComponents(GameObject target) {
			return target.GetComponentsInChildren<Renderer>();
		}
		public override void SetEnable(Component component, bool enable) {
			(component as Renderer).enabled = enable;
		}
		public override bool IsEnable(Component component) {
			return (component as Renderer).enabled;
		}
	}
}