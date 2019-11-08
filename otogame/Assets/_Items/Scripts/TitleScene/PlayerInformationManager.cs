using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInformationManager : SingletonMonoBehaviour<PlayerInformationManager> 
{
    private void Start()
    {
        DontDestroyOnLoad(this);
    }
    
    
    
}
