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
using UnityEngine.EventSystems;

namespace DM {

	public class UILayerTouchListener : UITouchListener, ICanvasRaycastFilter {

		Vector2 m_screenPoint = Vector2.zero;
		bool m_pressed = false;
		bool m_raycasted = false;

		public bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera) {
			m_screenPoint = screenPoint;
			m_raycasted = true;
			return false;
		}

		public void Update() {
			if (!m_raycasted || !layer.IsTouchable()) {
				m_pressed = false;
				return;
			}

			if (Input.touchCount > 0) {
				if (Input.touches[0].phase == TouchPhase.Began) {
					m_pressed = true;
					base.OnPointerDown(CreatePointerEventData());
					return;
				} else if (Input.touches[0].phase == TouchPhase.Ended) {
					if (!m_pressed) { return; }
					m_pressed = false;
					base.OnPointerUp(CreatePointerEventData());
					return;
				}
			}

			if (Input.GetMouseButtonDown(0)) {
				m_pressed = true;
				base.OnPointerDown(CreatePointerEventData());
				return;
			} else if (Input.GetMouseButtonUp(0)) {
				if (!m_pressed) { return; }
				m_pressed = false;
				base.OnPointerUp(CreatePointerEventData());
				return;
			}

			if (m_pressed) {
				base.OnDrag(CreatePointerEventData());
			}
		}

		public void LateUpdate() {
			m_raycasted = false;
		}

		public PointerEventData CreatePointerEventData() {
			PointerEventData data = new PointerEventData(null);
			data.position = m_screenPoint;
			return data;
		}
	}
}