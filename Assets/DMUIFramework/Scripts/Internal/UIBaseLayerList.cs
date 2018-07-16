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
	public class UIBaseLayerList {
		private List<UIBaseLayer> m_list = new List<UIBaseLayer>();

		public static int GetNumInGroup(UIGroup group, List<UIBaseLayer> list) {
			int count = 0;
			for (int i = 0; i < list.Count; i++) {
				if (group == list[i].ui.group) { ++count; }
			}
			return count;
		}

		public int GetNumInGroup(UIGroup group) {
			return UIBaseLayerList.GetNumInGroup(group, m_list);
		}

		public int Count { get{ return m_list.Count; } }

		public void Insert(UIBaseLayer layer) {
			int index = FindInsertPosition(layer.ui.group);
			if (index < 0) {
				m_list.Add(layer);
			} else {
				m_list.Insert(index, layer);
			}
		}

		public void Eject(UIBaseLayer layer) {
			int index = m_list.FindIndex(l => {
				return (l == layer);
			});
			if (index >= 0) {
				m_list.RemoveAt(index);
			}
		}

		public void ForEachOnlyActive(Action<UIBaseLayer> action) {
			List<UIBaseLayer> list = new List<UIBaseLayer>(m_list);
			for (int i = 0; i < list.Count; i++) {
				if (list[i].state == State.Active) {
					action(list[i]);
				}
			}
		}

		public void ForEachAnything(Action<UIBaseLayer> action) {
			List<UIBaseLayer> list = new List<UIBaseLayer>(m_list);
			for (int i = 0; i < list.Count; i++) {
				action(list[i]);
			}
		}

		public UIBaseLayer Find(UIBase ui) {
			return m_list.Find(l => {
				return (l.ui == ui);
			});
		}

		public List<UIBaseLayer> FindLayers(UIGroup group) {
			List<UIBaseLayer> list = new List<UIBaseLayer>();
			for (int i = 0; i < m_list.Count; i++) {
				if (m_list[i].ui.group == group) {
					list.Add(m_list[i]);
				}
			}
			return list;
		}

		public UIBaseLayer FindFrontLayerInGroup(UIGroup group) {
			return m_list.Find(l => {
				return (l.ui.group == group);
			});
		}

		public bool Has(string name) {
			return m_list.Exists(l => {
				return (l.ui.name == name);
			});
		}

		private int FindInsertPosition(UIGroup group) {
			if (group == UIGroup.None) { return -1; }

			int index = FindFrontIndexInGroup(group);
			if (index > -1) { return index; }

			return FindInsertPosition(group - 1);
		}

		private int FindFrontIndexInGroup(UIGroup group) {
			return m_list.FindIndex(l => {
				return (l.ui.group == group);
			});
		}
	}
}
