using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInformationManager : SingletonMonoBehaviour<PlayerInformationManager>
{
    public string[] name = new string[2];
    public float[] hispeed = new float[2];

    private void Start()
    {
        DontDestroyOnLoad(this);
    }
    
    
    
}
