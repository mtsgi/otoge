using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicSelectManager : SingletonMonoBehaviour<MusicSelectManager> 
{
    [Serializable]
    public class KeyInputSetting
    {
        public IndexJsonReadManager targetJsonReadManager;
        public KeyCode[] KeyCodes;
        public Animator[] frameWaveAnimController;
    }
    
    [SerializeField] private List<KeyInputSetting> KeyInputSettings;

    private void Update()
    {
        if (Input.anyKey)
        {
            for (int i = 0; i < KeyInputSettings.Count; i++)
            {
                if (Input.GetKeyDown(KeyInputSettings[i].KeyCodes[0]))
                {
                    KeyInputSettings[i].frameWaveAnimController[0].Play("Wave", 0, 0.0f);
//                    KeyInputSettings[i].targetJsonReadManager.ScroolUp();
                    
                    Debug.Log("上");
                }
                else if (Input.GetKeyDown(KeyInputSettings[i].KeyCodes[1]))
                {
                    KeyInputSettings[i].frameWaveAnimController[1].Play("Wave", 0, 0.0f);
 //                   KeyInputSettings[i].targetJsonReadManager.ScroolDown();

                    Debug.Log("下");
                }
                else if (Input.GetKeyDown(KeyInputSettings[i].KeyCodes[2]))
                {
                    KeyInputSettings[i].frameWaveAnimController[2].Play("Wave", 0, 0.0f);
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
