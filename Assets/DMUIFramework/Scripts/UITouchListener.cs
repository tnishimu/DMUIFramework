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

	public enum TouchType {
		None = 0,
		Click,
		Down,
		Up,
		Drag,
	}

	public class UITouchListener
	: MonoBehaviour
	, IPointerClickHandler
	, IPointerDownHandler
	, IPointerUpHandler
	, IDragHandler
	{
		private UIBaseLayer m_layer;
		public UIBaseLayer layer { get { return m_layer; } }

		private UIPart m_ui = null;
		public UIPart ui { get { return m_ui; } }

		private int m_generation = int.MaxValue;

		public void SetUI(UIBaseLayer layer, UIPart ui) {
			int generation = GetGeneration(transform, ui.root);
			if (m_generation < generation) {
				return;
			}

			m_layer = layer;
			m_ui = ui;
			m_generation = generation;
		}

		public void ResetUI() {
			m_layer = null;
			m_ui = null;
			m_generation = int.MaxValue;
		}

		public void OnPointerClick(PointerEventData pointer) {
			UIController.instance.ListenTouch(this, TouchType.Click, pointer);
		}

		public void OnPointerDown(PointerEventData pointer) {
			UIController.instance.ListenTouch(this, TouchType.Down, pointer);
		}

		public void OnPointerUp(PointerEventData pointer) {
			UIController.instance.ListenTouch(this, TouchType.Up, pointer);
		}

		public void OnDrag(PointerEventData pointer) {
			UIController.instance.ListenTouch(this, TouchType.Drag, pointer);
		}

		private int GetGeneration(Transform target, Transform dest, int generation = 0) {
			if (target == null || dest == null) {
				return -1;
			} else if (target == dest) {
				return generation;
			} else {
				return GetGeneration(target.parent, dest, generation+1);
			}
		}
	}
}