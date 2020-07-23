using UnityEngine;

[CreateAssetMenu]
public class MusicDataObject : ScriptableObject
{
    //シーンまたがって渡したい値たち
    public string jsonFilePath = "otofuda/otofuda/otofuda";
    public float bpm = 60;
    public float beat = 4.0f;
    public int offset = 0;
    public GameDifficulty[] levels = {GameDifficulty.Hard, GameDifficulty.Normal};
    public string musicId = "otofuda";

    public MusicData GetMusicData()
    {
        var musicData = new MusicData();
        musicData.jsonFilePath = jsonFilePath;
        musicData.bpm = bpm;
        musicData.beat = beat;
        musicData.levels = levels;
        musicData.musicId = musicId;
        musicData.offset = offset;

        return musicData;
    }
}
