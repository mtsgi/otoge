using System.Collections;
using System.Collections.Generic;
using OtoFuda.RythmSystem;
using UnityEngine;

public class test : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		Debug.Log(SoundManager.Instance._audioSources.Length);
		if (Input.GetKeyDown(KeyCode.A))
		{
			
		}
	}
}
