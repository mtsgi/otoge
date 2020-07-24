using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AutoPlaySetting
{
    public string jsonFilePath ="otofuda/otofuda/otofuda";
    public string musicFilePath = "otofuda";
    
    public float bpm = 180;
    public float beat = 4.0f;
    public int offset = 0;
    public GameDifficulty[] levels = {GameDifficulty.Hard, GameDifficulty.Normal};
}
