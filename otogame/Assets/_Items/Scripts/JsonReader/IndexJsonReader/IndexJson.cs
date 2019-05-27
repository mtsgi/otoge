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
		public string name = string.Empty;
		public string artist = String.Empty;
		public float bpm = 0.0f;
		public int[] color = new int[3]; 
		public int offset = 1000;
		public int raku = -1;
		public int easy = 1;
		public int normal= 3;
		public int hard= 7;
		public int extra= -1;
		public string author = string.Empty;
		public string comment = string.Empty;
	}
}

