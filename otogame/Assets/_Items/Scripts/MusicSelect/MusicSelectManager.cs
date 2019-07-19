using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicSelectManager : SingletonMonoBehaviour<MusicSelectManager> 
{
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

    private void Update()
    {
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
                    
                    Debug.Log("上");
                }
                else if (Input.GetKeyDown(KeyInputSettings[i].KeyCodes[1]))
                {
                    
                    if (isSelectMusic)
                    {
                        KeyInputSettings[i].frameWaveAnimController[1].Play("Wave", 0, 0.0f);
                        
                        KeyInputSettings[0].targetJsonReadManager.Escape();
                        KeyInputSettings[0].UserInterFaceTexts[1].text = "下";
                        
                        KeyInputSettings[1].targetJsonReadManager.Escape();
                        KeyInputSettings[1].UserInterFaceTexts[1].text = "下";
                        
                        isSelectMusic = false;

                        return;
                    }
                    
                    
                    KeyInputSettings[i].frameWaveAnimController[1].Play("Wave", 0, 0.0f);
                    KeyInputSettings[i].targetJsonReadManager.ScrollDown();
                    
                    Debug.Log("下");
                }
                else if (Input.GetKeyDown(KeyInputSettings[i].KeyCodes[2]))
                {
                    KeyInputSettings[i].frameWaveAnimController[2].Play("Wave", 0, 0.0f);
                   
                    //曲が選択されたフラグを立てる
                    isSelectMusic = true;
                    //下キーを戻るキーとして認識することにする
                    KeyInputSettings[0].UserInterFaceTexts[1].text = "戻";
                    KeyInputSettings[0].targetJsonReadManager.SelectMusic();

                    KeyInputSettings[1].UserInterFaceTexts[1].text = "戻";
                    KeyInputSettings[1].targetJsonReadManager.SelectMusic();
                    

                    Debug.Log("決定");
                }
                else if (Input.GetKeyDown(KeyInputSettings[i].KeyCodes[3]))
                {
                    KeyInputSettings[i].frameWaveAnimController[3].Play("Wave", 0, 0.0f);
                    Debug.Log("左");
                }
                else if (Input.GetKeyDown(KeyInputSettings[i].KeyCodes[4]))
                {
                    KeyInputSettings[i].frameWaveAnimController[4].Play("Wave", 0, 0.0f);
                    Debug.Log("右");
                }
            }
            
        }
    }
}
