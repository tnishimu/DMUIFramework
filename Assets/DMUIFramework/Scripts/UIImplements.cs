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

	public class PrefabReceiver {
		public UnityEngine.Object prefab;
	}
	public interface IPrefabLoader {
		IEnumerator Load(string path, PrefabReceiver receiver);
		void Release(string path, UnityEngine.Object prefab);
	}

	public interface ISounder {
		void PlayDefaultClickSE();
		void PlayClickSE(string name);
		void PlayBGM(string name);
		void StopBGM();
	}

	public interface IFadeCreator {
		UIFade Create();
	}

	public class UIImplements {
		private IPrefabLoader m_prefabLoader;
		public IPrefabLoader prefabLoader { get { return m_prefabLoader; } }

		private ISounder m_sounder;
		public ISounder sounder { get { return m_sounder; } }

		private IFadeCreator m_fadeCreator;
		public IFadeCreator fadeCreator { get { return m_fadeCreator; } }

		public UIImplements(IPrefabLoader prefabLoader, ISounder sounder, IFadeCreator fadeCreator) {
			m_prefabLoader = prefabLoader;
			m_sounder = sounder;
			m_fadeCreator = fadeCreator;
		}
	}
}