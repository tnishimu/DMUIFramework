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
using UnityEngine.EventSystems;

namespace DM {

	public class UIController : MonoBehaviour {

		public readonly static string LayerTouchAreaName = "LayerTouchArea";

		public GameObject[] m_raycasters;
		public Transform m_uiLayers;
		public Transform m_view3D;

		private List<BaseRaycaster> m_raycasterComponents = new List<BaseRaycaster>();

		private List<UIBaseLayer> m_addingList = new List<UIBaseLayer>();
		private List<UIBaseLayer> m_removingList = new List<UIBaseLayer>();
		private UIBaseLayerList m_uiList = new UIBaseLayerList();

		private Queue<TouchEvent> m_touchEvents = new Queue<TouchEvent>();
		private Queue<DispatchedEvent> m_dispatchedEvents = new Queue<DispatchedEvent>();
		private int m_touchOffCount;

		private UIBaseLayer m_uiFade;

		private UIImplements m_implements;
		public static UIImplements implements { get { return m_instance.m_implements; } }
		public void Implement(IPrefabLoader prefabLoader, ISounder sounder, IFadeCreator fadeCreator) {
			m_instance.m_implements = new UIImplements(prefabLoader, sounder, fadeCreator);
		}

		private static UIController m_instance;
		public static UIController instance {
			get {
				if (m_instance == null) {
					m_instance = GameObject.Find("DMUICanvas").GetComponent<UIController>();

					for (int i = 0; i < m_instance.m_raycasters.Length; i++) {
						BaseRaycaster raycaster = m_instance.m_raycasters[i].GetComponent<BaseRaycaster>();
						m_instance.m_raycasterComponents.Add(raycaster);
					}

					UIBackable.Sort();
				}
				return m_instance;
			}
		}

		public void AddFront(UIBase ui) {
			if (ui == null) { return; }

			UIBaseLayer layer = new UIBaseLayer(ui, m_uiLayers);

			if (layer.ui.LoadingWithoutFade()) {
				StartCoroutine(layer.Load());
			}

			if (ShouldFadeByAdding(ui)) {
				FadeIn();
			}

			m_addingList.Add(layer);
			m_uiList.Insert(layer);
		}

		public void Remove(UIBase ui) {
			if (ui == null) { return; }

			UIBaseLayer layer = m_uiList.Find(ui);
			if (layer != null && layer.Inactive()) {
				m_removingList.Add(layer);
			}

			if (ShouldFadeByRemoving(ui)) {
				FadeIn();
			}
		}

		public void Replace(UIBase[] uis, UIGroup[] removeGroups = null) {
			HashSet<UIGroup> removes = (removeGroups == null) ? new HashSet<UIGroup>() : new HashSet<UIGroup>(removeGroups);

			for (int i = 0; i < uis.Length; i++) {
				removes.Add(uis[i].group);
			}

			foreach (UIGroup group in removes) {
				List<UIBaseLayer> layers = m_uiList.FindLayers(group);
				for (int i = 0; i < layers.Count; i++) {
					Remove(layers[i].ui);
				}
			}

			for (int i = 0; i < uis.Length; i++) {
				AddFront(uis[i]);
			}
		}

		public void ListenTouch(UITouchListener listener, TouchType type, PointerEventData pointer) {
			if (listener.layer == null) { return; }

			m_touchEvents.Enqueue(new TouchEvent(listener, type, pointer));
		}

		public void Dispatch(string name, object param) {
			m_dispatchedEvents.Enqueue(new DispatchedEvent(name, param));
		}

		public void Back() {
			UIBaseLayer layer = null;
			for (int i = 0; i < UIBackable.groups.Count; i++) {
				layer = m_uiList.FindFrontLayerInGroup(UIBackable.groups[i]);
				if (layer != null) { break; }
			}
			if (layer == null) { return; }

			bool ret = layer.ui.OnBack();
			if (ret) {
				Remove(layer.ui);
			}
		}

		public IEnumerator YieldAttachParts(UIBase uiBase, List<UIPart> parts) {
			UIBaseLayer layer = m_uiList.Find(uiBase);
			if (layer == null) { yield break; }
			yield return layer.AttachParts(parts);
		}

		public void AttachParts(UIBase uiBase, List<UIPart> parts) {
			UIBaseLayer layer = m_uiList.Find(uiBase);
			if (layer == null) { return; }
			StartCoroutine(layer.AttachParts(parts));
		}

		public void DetachParts(UIBase uiBase, List<UIPart> parts) {
			UIBaseLayer layer = m_uiList.Find(uiBase);
			if (layer == null) { return; }
			layer.DetachParts(parts);
		}

		public void SetScreenTouchable(UIBase uiBase, bool enable) {
			UIBaseLayer layer = m_uiList.Find(uiBase);
			if (layer == null) { return; }
			SetScreenTouchableByLayer(layer, enable);
		}

		public void SetScreenTouchableByLayer(UIBaseLayer layer, bool enable) {
			if (layer == null) { return; }

			if (enable) {
				if (m_touchOffCount <= 0) { return; }
				--m_touchOffCount;
				--layer.screenTouchOffCount;
				if (m_touchOffCount == 0) {
					for (int i = 0; i < m_raycasterComponents.Count; i++) {
						m_raycasterComponents[i].enabled = true;
					}
				}
			} else {
				if (m_touchOffCount == 0) {
					for (int i = 0; i < m_raycasterComponents.Count; i++) {
						m_raycasterComponents[i].enabled = false;
					}
				}
				++m_touchOffCount;
				++layer.screenTouchOffCount;
			}
		}

		public bool HasUI(string name) {
			return m_uiList.Has(name);
		}

		public string GetFrontUINameInGroup(UIGroup group) {
			UIBaseLayer layer = m_uiList.FindFrontLayerInGroup(group);
			if (layer == null) {
				return "";
			} else {
				return layer.ui.name;
			}
		}

		public int GetUINumInGroup(UIGroup group) {
			return m_uiList.GetNumInGroup(group);
		}

		private void Update() {
			m_uiList.ForEachOnlyActive(layer => {
				if (layer.ui.scheduleUpdate) {
					layer.ui.OnUpdate();
				}
			});

			RunTouchEvents();
			RunDispatchedEvents();

			bool isInsert = Insert();
			bool isEject  = Eject();
			if (isEject || isInsert) {
				RefreshLayer();

				if (isEject && IsHidden()) {
					Unload();
				}
				if (m_addingList.Count == 0 && m_removingList.Count == 0) {
					PlayBGM();
					FadeOut();
				}
			}
		}

		private void LateUpdate() {
			m_uiList.ForEachOnlyActive(layer => {
				if (layer.ui.scheduleUpdate) {
					layer.ui.OnLateUpdate();
				}
			});
		}

		private void OnDestroy() {
			UIController.m_instance = null;
		}

		private bool Insert() {
			bool isInsert = false;

			if (m_addingList.Count <= 0) { return isInsert; }

			List<UIBaseLayer> list = m_addingList;
			m_addingList = new List<UIBaseLayer>();
			bool isFadeIn = IsFadeIn();
			for (int i = 0; i < list.Count; i++) {
				UIBaseLayer layer = list[i];
				if (!isFadeIn && layer.state == State.InFading) {
					StartCoroutine(layer.Load());
				}
				if (layer.IsNotYetLoaded() || (isFadeIn && !layer.ui.ActiveWituoutFade())) {
					m_addingList.Add(layer);
					continue;
				}

				if (layer.Activate()) {
					isInsert = true;
				}
			}

			return isInsert;
		}

		private bool Eject() {
			bool isEject = false;

			if (m_removingList.Count <= 0) { return isEject; }

			bool isLoading = m_addingList.Exists(layer => {
				return (layer.IsNotYetLoaded());
			});

			List<UIBaseLayer> list = m_removingList;
			m_removingList = new List<UIBaseLayer>();
			bool isFadeIn = IsFadeIn();
			for (int i = 0; i < list.Count; i++) {
				UIBaseLayer layer = list[i];
				if (!isFadeIn && layer.state == State.OutFading) {
					layer.Remove();
				}

				if (layer.state != State.Removing || isLoading) {
					m_removingList.Add(layer);
					continue;
				}

				m_uiList.Eject(layer);
				layer.Destroy();
				isEject = true;
			}

			return isEject;
		}

		private void RefreshLayer() {
			bool visible   = true;
			bool touchable = true;
			UIBaseLayer frontLayer = null;
			int index = m_uiLayers.childCount - 1;
			m_uiList.ForEachAnything(layer => {
				if (layer.IsNotYetLoaded()) { return; }

				bool preVisible   = layer.IsVisible();
				bool preTouchable = layer.IsTouchable();
				layer.SetVisible  (visible);
				layer.SetTouchable(touchable);
				if (!preVisible   && visible  ) { layer.ui.OnRevisible();   }
				if (!preTouchable && touchable) { layer.ui.OnRetouchable(); }

				visible   = visible   && layer.ui.BackVisible();
				touchable = touchable && layer.ui.BackTouchable();

				layer.siblingIndex = index--;

				if (frontLayer != null) {
					frontLayer.back = layer;
					frontLayer.CallSwitchBack();
				}
				layer.front = frontLayer;
				layer.CallSwitchFront();

				layer.back = null;
				frontLayer = layer;
			});
		}

		private void RunTouchEvents() {
			if (m_touchEvents.Count == 0) { return; }

			bool ret = false;
			int untouchableIndex = FindUntouchableIndex();

			Queue<TouchEvent> queue = new Queue<TouchEvent>(m_touchEvents);
			m_touchEvents.Clear();

			while (queue.Count > 0) {
				TouchEvent touch = queue.Dequeue();

				if (ret) { continue; }
				if (touch.listener.layer == null) { continue; }

				bool touchable = true;
				touchable = touchable && IsScreentouchable();
				touchable = touchable && touch.listener.layer.IsTouchable();
				touchable = touchable && untouchableIndex < touch.listener.layer.siblingIndex;
				if (!touchable) { continue; }

				UIPart ui = touch.listener.ui;
				switch (touch.type) {
					case TouchType.Click: {
						UIPart.SE se = new UIPart.SE();
						ret = ui.OnClick(touch.listener.gameObject.name, touch.listener.gameObject, touch.pointer, se);
						if (ret && m_implements.sounder != null) {
							if (!string.IsNullOrEmpty(se.playName)) {
								m_implements.sounder.PlayClickSE(se.playName);
							} else {
								m_implements.sounder.PlayDefaultClickSE();
							}
						}
						break;
					}
					case TouchType.Down: {
						ret = ui.OnTouchDown(touch.listener.gameObject.name, touch.listener.gameObject, touch.pointer);
						break;
					}
					case TouchType.Up: {
						ret = ui.OnTouchUp(touch.listener.gameObject.name, touch.listener.gameObject, touch.pointer);
						break;
					}
					case TouchType.Drag: {
						ret = ui.OnDrag(touch.listener.gameObject.name, touch.listener.gameObject, touch.pointer);
						break;
					}
					default: break;
				}
			}
		}

		private void RunDispatchedEvents() {
			if (m_dispatchedEvents.Count == 0) { return; }

			Queue<DispatchedEvent> queue = new Queue<DispatchedEvent>(m_dispatchedEvents);
			m_dispatchedEvents.Clear();

			while (queue.Count > 0) {
				DispatchedEvent e = queue.Dequeue();
				m_uiList.ForEachOnlyActive(layer => {
					layer.ui.OnDispatchedEvent(e.name, e.param);
				});
			}
		}

		private bool ShouldFadeByAdding(UIBase ui) {
			if (m_uiFade != null) { return false; }

			if (UIFadeTarget.groups.Contains(ui.group)) { return true; }

			bool has = UIFadeThreshold.groups.ContainsKey(ui.group);
			if (has && m_uiList.GetNumInGroup(ui.group) <= UIFadeThreshold.groups[ui.group]) {
				return true;
			}

			return false;
		}

		private bool ShouldFadeByRemoving(UIBase ui) {
			if (m_uiFade != null) { return false; }

			if (UIFadeTarget.groups.Contains(ui.group)) { return true; }

			bool has = UIFadeThreshold.groups.ContainsKey(ui.group);
			if (has) {
				int sceneNum = UIBaseLayerList.GetNumInGroup(ui.group, m_removingList);
				if (m_uiList.GetNumInGroup(ui.group) - sceneNum <= UIFadeThreshold.groups[ui.group]) {
					return true;
				}
			}

			return false;
		}

		private void FadeIn() {
			if (m_uiFade != null) { return; }

			if (m_implements.fadeCreator == null) { return; }

			UIFade fade = m_implements.fadeCreator.Create();
			AddFront(fade);
			m_uiFade = m_addingList.Find(l => { return l.ui == fade; });
		}

		private void FadeOut() {
			if (m_uiFade == null) { return; }

			Remove(m_uiFade.ui);
			m_uiFade = null;
		}

		private bool IsFadeIn() {
			return (m_uiFade != null && m_uiFade.state <= State.InAnimation);
		}

		private bool IsHidden() {
			return (m_uiFade != null && m_uiFade.state == State.Active);
		}

		private bool IsScreentouchable() {
			if (m_raycasterComponents.Count == 0) { return false; }

			return m_raycasterComponents[0].enabled;
		}

		private int FindUntouchableIndex() {
			int index = -1;
			m_uiList.ForEachOnlyActive(layer => {
				if (index >= 0) { return; }
				if (layer.ui.SystemUntouchable()) {
					index = layer.siblingIndex - 1;
				}
			});
			return index;
		}

		private void Unload() {
			System.GC.Collect();
			Resources.UnloadUnusedAssets();
		}

		private void PlayBGM() {
			if (m_implements.sounder == null) { return; }

			string bgm = "";
			m_uiList.ForEachAnything(l => {
				if (!StateFlags.map[l.state].visible) { return; }
				if (!string.IsNullOrEmpty(bgm)) { return; }
				if (!string.IsNullOrEmpty(l.ui.bgm)) {
					bgm = l.ui.bgm;
				}
			});

			if (string.IsNullOrEmpty(bgm)) {
				m_implements.sounder.StopBGM();
			} else {
				m_implements.sounder.PlayBGM(bgm);
			}
		}
	}
}