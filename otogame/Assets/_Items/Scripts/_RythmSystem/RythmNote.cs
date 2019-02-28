using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OtoFuda.RythmSystem
{
	public class RythmNote : MonoBehaviour,IRythmNote
	{
		private AudioSource[] _audioSources;
		// Use this for initialization
		void Start ()
		{
			//SoundManagerからAudiosouceを拾ってくる
			_audioSources = SoundManager.Instance._audioSources;
		}
	
		// Update is called once per frame
		void Update () 
		{
			//testKey();
		}

		//判定を受けとる部分
		public void getJudge(int judge)
		{
			//judgeに渡された値で精度を判定
			//値でプレイヤー同士が殴り合うので4->0の順
			switch (judge)
			{
				case 4:
					_audioSources[4].Play();
					Debug.Log("Perfect");
					break;
				case 3:
					_audioSources[3].Play();
					Debug.Log("Great");
					break;
				case 2:
					_audioSources[2].Play();
					Debug.Log("Good");
					break;
				case 1:
					_audioSources[1].Play();
					Debug.Log("Bad");
					break;
				case 0:
					_audioSources[0].Play();
					Debug.Log("Miss");
					break;
				default:
					Debug.LogError("Mis match Type Enum");
					break;
					
			}
		}


		private void testKey()
		{
			//キー入力テスト
			if (Input.GetKeyDown(KeyCode.A))
			{
				getJudge(4);
			}
			if (Input.GetKeyDown(KeyCode.B))
			{
				getJudge(3);
			}
			if (Input.GetKeyDown(KeyCode.C))
			{
				getJudge(2);
			}
			if (Input.GetKeyDown(KeyCode.D))
			{
				getJudge(1);
			}
			if (Input.GetKeyDown(KeyCode.E))
			{
				getJudge(0);
			}
		}
		
	}	

}
