using System.Collections;
using System.Collections.Generic;
using OtoFuda.RythmSystem;
using UnityEngine;

public class UserKeyInputManager : MonoBehaviour {
    public KeyCode Key;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (Input.GetKeyDown(Key))
		{
            SoundManager.Instance.playSound(0).Play();
        }
	}
}
