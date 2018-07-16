// ----------------------------------------------------------------------
// DMUIFramework
// Copyright (c) 2018 Takuya Nishimura (tnishimu)
//
// This software is released under the MIT License.
// https://opensource.org/licenses/mit-license.php
// ----------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace DM {

	public class TouchEvent {
		private UITouchListener m_listener;
		public UITouchListener listener { get { return m_listener; } }

		private TouchType m_type;
		public TouchType type { get { return m_type; } }

		private PointerEventData m_pointer;
		public PointerEventData pointer { get{ return m_pointer; } }

		public TouchEvent(UITouchListener listener, TouchType type, PointerEventData pointer) {
			m_listener = listener;
			m_type = type;
			m_pointer = pointer;
		}
	}

	public class DispatchedEvent {
		private string m_name;
		public string name { get { return m_name; } }

		private object m_param;
		public object param { get { return m_param; } }

		public DispatchedEvent(string name, object param) {
			m_name = name;
			m_param = param;
		}
	}
}