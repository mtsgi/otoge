using System;
using UnityEngine;

[Serializable]
public class MusicData
{
    //シーンまたがって渡したい値たち
    public string jsonFilePath = "otofuda/otofuda/otofuda";
    public float bpm = 60;
    public float beat = 4.0f;
    public int offset = 0;

    public GameDifficulty[] levels = {GameDifficulty.Hard, GameDifficulty.Normal};
    public string musicId = "otofuda";

    public void TestCheckParameter()
    {
        Debug.Log($"MusicData Info :FilePath[{jsonFilePath}]/BPM[{bpm}]" +
                  $"/beat[{beat}]/musicID[{musicId}]");
    }
}