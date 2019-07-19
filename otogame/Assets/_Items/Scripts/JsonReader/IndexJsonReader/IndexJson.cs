using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OtoFuda.Fumen.index
{
	[Serializable]
	public class IndexInfo
	{
		public List<IndexContent> index = null;
	}
	
	
	[Serializable]
	public class IndexContent
	{
        public string id = string.Empty; // 楽曲ID
		public string name = string.Empty;
		public string artist = String.Empty;
		public float bpm = 0.0f;
        public string dispbpm = string.Empty; // 表記BPM
		public int[] color = new int[3];
		public int offset = 1000;
		public int raku = -1;
		public int easy = 1;
		public int normal= 3;
		public int hard= 7;
		public int extra= -1;
		public string author = string.Empty;
		public string comment = string.Empty;
        public float demo = 0.0f; // 楽曲デモ開始時間
	}
}

