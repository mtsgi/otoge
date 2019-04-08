using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OtoFuda.RythmSystem
{

	public class Note
	{
		//譜面の中で何個目のノーツか
		public int noteNumber;
		
		//ノーツが何番目のレーンにあるか
		public int lane;
		
		//ノーツの種類
		//0~nで表す。
		//0がSingle,1がLong,2がOtofuda
		public int noteTypeNumber;
		
	}
	

}