using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.UI;

public class MusicSelectManager : SingletonMonoBehaviour<MusicSelectManager> 
{
    [Serializable]
    public class MusicData : ISceneTransitionData
    {
        //シーンまたがって渡したい値たち
        public string jsonFilePath = "puzzle/puzzle/puzzle";
        public float BPM = 60;
        public float[] HISPEED = {1.0f, 1.0f};
        public JsonReadManager.DIFFICULTY[] LEVELS = {JsonReadManager.DIFFICULTY.NORMAL, JsonReadManager.DIFFICULTY.NORMAL};
        public string musicID;
        
        public void TestCheckParameter()
        {
            Debug.Log(
                $"MusicData Info :FilePath{jsonFilePath}/BPM{BPM}/HiSpeed{HISPEED}/Level{LEVELS[0]}/musicID{musicID}");
        }
    }

    public MusicData musicData = new MusicData();

    [Serializable]
    public class KeyInputSetting
    {
        public IndexJsonReadManager targetJsonReadManager;
        public KeyCode[] KeyCodes;
        public Animator[] frameWaveAnimController;
        public Text[] UserInterFaceTexts;
    }
    
    [SerializeField] private List<KeyInputSetting> KeyInputSettings;
    private bool isSelectMusic = false;

    private int selectPlayerID;

    private void Update()
    {
        if (KeyInputSettings[0].targetJsonReadManager.isSelectLevel &&
            KeyInputSettings[1].targetJsonReadManager.isSelectLevel)
        {
            KeyInputSettings[0].targetJsonReadManager.LoadScene(KeyInputSettings[selectPlayerID].targetJsonReadManager.focus);
        }

        if (Input.anyKey)
        {
            for (int i = 0; i < KeyInputSettings.Count; i++)
            {
                if (Input.GetKeyDown(KeyInputSettings[i].KeyCodes[0]))
                {
                    if (isSelectMusic)
                    {
                        return;
                    }
                    KeyInputSettings[i].frameWaveAnimController[0].Play("Wave", 0, 0.0f);
                    KeyInputSettings[i].targetJsonReadManager.ScrollUp();
                    
/*
                    Debug.Log("上");
*/
                }
                else if (Input.GetKeyDown(KeyInputSettings[i].KeyCodes[1]))
                {
                    if (isSelectMusic)
                    {
                        KeyInputSettings[i].frameWaveAnimController[1].Play("Wave", 0, 0.0f);
                        
                        KeyInputSettings[0].targetJsonReadManager.Escape();
                        KeyInputSettings[0].UserInterFaceTexts[1].text = "下";
                        KeyInputSettings[0].UserInterFaceTexts[3].text = "遅";
                        KeyInputSettings[0].UserInterFaceTexts[4].text = "速";
                        
                        KeyInputSettings[1].targetJsonReadManager.Escape();
                        KeyInputSettings[1].UserInterFaceTexts[1].text = "下";
                        KeyInputSettings[1].UserInterFaceTexts[3].text = "遅";
                        KeyInputSettings[1].UserInterFaceTexts[4].text = "速";
                        
                        isSelectMusic = false;

                        return;
                    }
                    
                    
                    KeyInputSettings[i].frameWaveAnimController[1].Play("Wave", 0, 0.0f);
                    KeyInputSettings[i].targetJsonReadManager.ScrollDown();
                    
/*
                    Debug.Log("下");
*/
                }
                else if (Input.GetKeyDown(KeyInputSettings[i].KeyCodes[2]))
                {
                    KeyInputSettings[i].frameWaveAnimController[2].Play("Wave", 0, 0.0f);

                    if (isSelectMusic)
                    {
                        KeyInputSettings[i].targetJsonReadManager.SelectMusic(KeyInputSettings[selectPlayerID].targetJsonReadManager.focus);
                        return;
                    }
                    
                    //誰の選んだ楽曲なのかを管理
                    selectPlayerID = i;
                    
                    //曲が選択されたフラグを立てる
                    isSelectMusic = true;
                    //下キーを戻るキーとして認識することにする
                    KeyInputSettings[0].UserInterFaceTexts[1].text = "戻";
                    KeyInputSettings[0].UserInterFaceTexts[3].text = "左";
                    KeyInputSettings[0].UserInterFaceTexts[4].text = "右";
                    KeyInputSettings[0].targetJsonReadManager.SelectMusic(KeyInputSettings[i].targetJsonReadManager.focus);

                    KeyInputSettings[1].UserInterFaceTexts[1].text = "戻";
                    KeyInputSettings[1].UserInterFaceTexts[3].text = "左";
                    KeyInputSettings[1].UserInterFaceTexts[4].text = "右";
                    KeyInputSettings[1].targetJsonReadManager.SelectMusic(KeyInputSettings[i].targetJsonReadManager.focus);
                    
/*
                    Debug.Log("決定");
*/
                }
                else if (Input.GetKeyDown(KeyInputSettings[i].KeyCodes[3]))
                {
                    KeyInputSettings[i].frameWaveAnimController[3].Play("Wave", 0, 0.0f);
                    KeyInputSettings[i].targetJsonReadManager.InputLeft();
/*
                    Debug.Log("左");
*/
                }
                else if (Input.GetKeyDown(KeyInputSettings[i].KeyCodes[4]))
                {
                    KeyInputSettings[i].frameWaveAnimController[4].Play("Wave", 0, 0.0f);
                    KeyInputSettings[i].targetJsonReadManager.InputRight();
/*
                    Debug.Log("右");
*/
                }
            }
            
        }
    }
}
