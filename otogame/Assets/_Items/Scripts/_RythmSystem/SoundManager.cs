using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OtoFuda.RythmSystem
{
	public class SoundManager : SingletonMonoBehaviour<SoundManager>
	{	
		[HideInInspector] public AudioSource[] _audioSources;
		void Awake()
		{
			base.Awake();
			_audioSources = GetComponents<AudioSource>();
			Debug.Log(_audioSources.Length);
		}


	}	

}

