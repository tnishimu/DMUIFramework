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
using UnityEngine.UI;

namespace DM {

	public enum State {
		None,           // 初期値
		// ↓ invisible, untouchable
		InFading,       // フェード終了待ち
		Loading,        // 読み込み待ち
		Adding,         // リストに追加待ち
		// ↓ visible
		InAnimation,    // 登場アニメーション中
		// ↓ screen touchable
		Active,         // 有効
		// ↓ screen untouchable
		OutAnimation,   // 退場アニメーション中
		OutFading,      // フェード終了待ち
		// ↓ invisible
		UselessLoading, // 無駄読み中
		Removing,       // リストから削除待ち
	}

	public class StateFlags {
		public readonly bool touchable;
		public readonly bool visible;

		public StateFlags(bool t, bool v) {
			touchable = t;
			visible   = v;
		}

		public static readonly Dictionary<State, StateFlags> map = new Dictionary<State, StateFlags> () {
			{ State.None          , new StateFlags (false, false) },
			{ State.InFading      , new StateFlags (false, false) },
			{ State.Loading       , new StateFlags (false, false) },
			{ State.Adding        , new StateFlags (false, false) },
			{ State.InAnimation   , new StateFlags (false, true ) },
			{ State.Active        , new StateFlags (true , true ) },
			{ State.OutAnimation  , new StateFlags (false, true ) },
			{ State.OutFading     , new StateFlags (false, true ) },
			{ State.UselessLoading, new StateFlags (false, false) },
			{ State.Removing      , new StateFlags (false, false) },
		};
	}

	public class UIBaseLayer : UIPartContainer {
		private State m_state = State.None;
		public State state { get { return m_state; } }

		private GameObject m_origin;
		private GameObject m_touchOff;
		private List<UIPartContainer> m_uiParts = new List<UIPartContainer>();

		private int m_screenTouchOffCount;
		public int screenTouchOffCount {
			get { return m_screenTouchOffCount; }
			set { m_screenTouchOffCount = value; }
		}
		private UIBaseLayer m_back;
		public UIBaseLayer back {
			get { return m_back; }
			set { m_back = value; }
		}
		private UIBaseLayer m_front;
		public UIBaseLayer front {
			get { return m_front; }
			set { m_front = value; }
		}

		private string m_linkedFrontName = "";
		private string m_linkedBackName = "";

		private Transform m_parent;

		public new UIBase ui { get { return (UIBase)base.ui; } }

		public UIBaseLayer(UIBase ui, Transform parent) : base(ui) {
			m_parent = parent;
			ProgressState(State.InFading);
		}

		public int siblingIndex {
			set { m_origin.transform.SetSiblingIndex(value); }
			get { return m_origin.transform.GetSiblingIndex(); }
 		}

		public bool IsNotYetLoaded() { return (m_state <= State.Loading || m_state == State.UselessLoading); }

		public override void Destroy() {
			for (int i = 0; i < screenTouchOffCount; i++) {
				UIController.instance.SetScreenTouchableByLayer(this, true);
			}
			if (m_origin != null) {
				m_origin.transform.SetParent(null);
				GameObject.Destroy(m_origin);
				m_origin = null;
			}

			for (int i = 0; i < m_uiParts.Count; i++) {
				m_uiParts[i].Destroy();
			}

			m_parent = null;

			base.Destroy();
		}

		public IEnumerator Load() {
			if (!ProgressState(State.Loading)) {
				ProgressState(State.Removing);
				yield break;
			}

			if (!string.IsNullOrEmpty(ui.prefabPath)) {
				PrefabReceiver receiver = new PrefabReceiver();
				yield return UIController.implements.prefabLoader.Load(ui.prefabPath, receiver);
				m_prefab = receiver.prefab;
			}

			m_origin = new GameObject(ui.name);
			SetupStretchAll(m_origin.AddComponent<RectTransform>());
			m_origin.transform.SetParent(m_parent, false);

			GameObject g = null;
			if (m_prefab != null) {
				g = GameObject.Instantiate(m_prefab) as GameObject;
				g.name = m_prefab.name;
			} else {
				g = new GameObject("root");
				SetupStretchAll(g.AddComponent<RectTransform>());
			}
			ui.root = g.transform;

			Transform parent = ui.View3D() ? UIController.instance.m_view3D : m_origin.transform;
			ui.root.SetParent(parent, false);
			ui.root.gameObject.SetActive(false);

			yield return ui.OnLoaded();
			Setup();

			if (m_state != State.Loading) {
				ProgressState(State.Removing);
				yield break;
			}

			ui.root.gameObject.SetActive(true);
			ProgressState(State.Adding);
		}

		public IEnumerator AttachParts(List<UIPart> parts) {
			if (m_state > State.Active) { yield break; }

			for (int i = 0; i < parts.Count; i++) {
				UIPartContainer container = new UIPartContainer(parts[i]);
				m_uiParts.Add(container);
				yield return container.LoadAndSetup(this);
			}
		}

		public void DetachParts(List<UIPart> parts) {
			if (m_state != State.Active) { return; }

			for (int i = 0; i < parts.Count; i++) {
				m_uiParts.RemoveAll(container => {
					return container.ui == parts[i];
				});
				parts[i].Destroy();
			}
		}

		public bool Activate() {
			if (m_state != State.Adding) {
				ExceptState();
				return false;
			}

			ProgressState(State.InAnimation);
			bool isPlay = ui.PlayAnimations("In", () => {
				ProgressState(State.Active);
			});
			if (!isPlay) {
				ProgressState(State.Active);
			}

			return true;
		}

		public bool Inactive() {
			if (this.state < State.Active) {
				this.ExceptState();
				return true;
			} else if (this.state > State.Active) {
				return false;
			}

			bool ret = ProgressState(State.OutAnimation);
			if (!ret) { return false; }

			bool isPlay = IsVisible();
			if (isPlay) {
				isPlay = ui.PlayAnimations("Out", () => {
					ProgressState(State.OutFading);
				}, true);
			}
			if (!isPlay) {
				ProgressState(State.OutFading);
			}

			return true;
		}

		public void Remove() {
			if (m_state == State.Removing || m_state == State.UselessLoading) { return; }

			if (m_state == State.Loading) {
				ProgressState(State.UselessLoading);
			} else {
				ProgressState(State.Removing);
			}
		}

		public void CallSwitchFront() {
			string pre = m_linkedFrontName;
			m_linkedFrontName = (this.front != null) ? this.front.ui.name : "";
			if (pre != m_linkedFrontName) {
				this.ui.OnSwitchFrontUI(m_linkedFrontName);
			}
		}

		public void CallSwitchBack() {
			string pre = m_linkedBackName;
			m_linkedBackName = (this.back != null) ? this.back.ui.name : "";
			if (pre != m_linkedBackName) {
				this.ui.OnSwitchBackUI(m_linkedBackName);
			}
		}

		public bool IsVisible() {
			if (m_origin == null) { return false; }

			if (ui.visibleControllers.Count <= 0) {
				return m_origin.activeSelf;
			} else {
				return ui.visibleControllers[0].IsVisible();
			}
		}

		public bool IsTouchable() {
			if (m_touchOff == null) { return false; }
			return !m_touchOff.activeSelf;
		}

		public void SetVisible(bool enable) {
			if (enable && !StateFlags.map[m_state].visible) { return; }
			if (m_origin == null) { return; }

			if (ui.visibleControllers.Count <= 0) {
				ui.root.gameObject.SetActive(enable);
			} else {
				for (int i = 0; i < ui.visibleControllers.Count; i++){
					ui.visibleControllers[i].SetVisible(ui.root.gameObject, enable);
				}
			}
		}

		public void SetTouchable(bool enable) {
			if (m_touchOff == null) { return; }
			m_touchOff.SetActive(!enable);
		}

		private bool CanVisible() {
			UIBaseLayer layer = m_front;
			while (layer != null) {
				if (!layer.ui.BackVisible()) { return false; }
				layer = layer.m_front;
			}
			return true;
		}

		private bool CanTouchable() {
			UIBaseLayer layer = m_front;
			while (layer != null) {
				if (!layer.ui.BackTouchable()) { return false; }
				layer = layer.m_front;
			}
			return true;
		}

		private void ExceptState() {
			Remove();
		}

		private bool ProgressState(State nextState) {
			if (m_state >= nextState) { return false; }

			m_state = nextState;
			StateFlags flags = StateFlags.map[nextState];

			if ((screenTouchOffCount == 0) != flags.touchable) {
				UIController.instance.SetScreenTouchableByLayer(this, flags.touchable);
			}

			if (flags.visible != IsVisible()) {
				if (!flags.visible || CanVisible()) {
					SetVisible(flags.visible);
				}
			}

			if (nextState == State.Active) {
				ui.OnActive();
			}

			return true;
		}

		private void Setup() {
			m_touchOff = CreateTouchPanel("LayerTouchOff");
			m_touchOff.SetActive(false);
			m_touchOff.transform.SetParent(m_origin.transform, false);

			GameObject touchArea = null;
			if (ui.TouchEventCallable()) {
				touchArea = CreateTouchPanel(UIController.LayerTouchAreaName);
				UILayerTouchListener listener = touchArea.AddComponent<UILayerTouchListener>();
				listener.SetUI(this, this.ui);
				touchArea.transform.SetParent(m_origin.transform, false);
			}
			GameObject systemTouchOff = null;
			if (ui.SystemUntouchable()) {
				systemTouchOff = CreateTouchPanel("SystemTouchOff");
				systemTouchOff.transform.SetParent(m_origin.transform, false);
			}

			List<GameObject> innerIndex = new List<GameObject>() {
				systemTouchOff,
				touchArea,
				ui.root.gameObject,
				m_touchOff,
			};
			int index = 0;
			for (int i = 0; i < innerIndex.Count; i++) {
				if (innerIndex[i] != null) { innerIndex[i].transform.SetSiblingIndex(index++); }
			}

			CollectComponents(ui.root.gameObject, this);
		}

		private GameObject CreateTouchPanel(string name) {
			GameObject gameObject = new GameObject(name);
			Image image = gameObject.AddComponent<Image>();
			image.color = new Color(0f, 0f, 0f, 0f);
			SetupStretchAll(gameObject.GetComponent<RectTransform>());
			return gameObject;
		}

		private void SetupStretchAll(RectTransform rectTransform) {
			rectTransform.anchorMin = Vector2.zero;
			rectTransform.anchorMax = Vector2.one;
			rectTransform.pivot     = Vector2.zero;
			rectTransform.sizeDelta = Vector2.zero;
		}
	}
}