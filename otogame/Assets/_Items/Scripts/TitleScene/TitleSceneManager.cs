using System;
using System.Collections;
using System.Collections.Generic;
using NfcPcSc;
using UnityEngine;


public class TitleSceneManager : MonoBehaviour
{
    [SerializeField] private TitleSceneUIUpdater _titleSceneUiUpdater;
    public List<PlayerTitleSceneInfo> playerTitleSceneInfos = new List<PlayerTitleSceneInfo>();
    public TitleSceneStatus status = TitleSceneStatus.StandBy;
    
    [Serializable]
    public class PlayerTitleSceneInfo
    {
        public KeyCode[] keys= new KeyCode[5];
        public bool isReady;

        public bool KeyCheck()
        {
            foreach (var t in keys)
            {
                if (Input.GetKeyDown(t))
                {
                    return true;
                }
            }

            return false;
        }
    }
    
    public enum TitleSceneStatus
    {
        StandBy,
        InvalidInput,

    }

    private void OnEnable()
    {
        NfcMain.OnNfcCardInput += OnNfcCardInput;
    }

    
    private void OnNfcCardInput(string nfcId)
    {
        status = TitleSceneStatus.InvalidInput;
        OtofudaNetAPIAccessManager.Instance.SendNfcId(nfcId);
    }


    public void Update()
    {
        if(status == TitleSceneStatus.InvalidInput) return;
        for (int i = 0; i < playerTitleSceneInfos.Count; i++)
        {
            if (playerTitleSceneInfos[i].KeyCheck())
            {
                _titleSceneUiUpdater.ActivatePlayerUI(i);
                status = TitleSceneStatus.InvalidInput;
            }
        }
    }
}
