using System;
using System.Collections;
using System.Collections.Generic;
using NfcPcSc;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;


public class TitleSceneManager : MonoBehaviour
{
    [SerializeField] private bool isTestMode;
    [SerializeField] private TitleSceneUIUpdater _titleSceneUiUpdater;
    public List<PlayerTitleSceneInfo> playerTitleSceneInfos = new List<PlayerTitleSceneInfo>();
    public TitleSceneStatus status = TitleSceneStatus.NfcInput;
    
    private bool tmpRegistered = false;
    private string tmpAccessCode = "";
    private string tmpName = "お名前";
    private float tmpHiSpeed =0.5f;
    private string tmpQECodeURI = "";


    private string underDefaultMessage;

    private bool isFinishUserDataInput = false;
    
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
        
        public int KeyCheckValue()
        {
            for (var index = 0; index < keys.Length; index++)
            {
                var t = keys[index];
                if (Input.GetKeyDown(t))
                {
                    return index;
                }
            }

            return -1;
        }
    }
    
    public enum TitleSceneStatus
    {
        StandBy,
        NfcInput,
        SelectPosition,
        WaitReady,
    }
    
    private void Start()
    {
        underDefaultMessage = _titleSceneUiUpdater.UnderMessageText.text; 
        status = TitleSceneStatus.NfcInput;

    }


    private void OnEnable()
    {
        NfcMain.OnNfcCardInput += OnNfcCardInput;
    }


    private async void OnNfcCardInput(string nfcId)
    {
        //読み取っても、NfcInputじゃなかったらなんもしない
        if(status != TitleSceneStatus.NfcInput) return;
        if (playerTitleSceneInfos[0].isReady && playerTitleSceneInfos[1].isReady) return;

        status = TitleSceneStatus.NfcInput;

        var jsonText = "";
        if (isTestMode)
        {
            jsonText = await OtofudaNetAPIAccessManager.Instance.SendNfcId(
                Guid.NewGuid().ToString("N").Substring(0, 10));
        }
        else
        {
            jsonText = await OtofudaNetAPIAccessManager.Instance.SendNfcId(nfcId);
        }
        
        var getData = new OtofudaAPIJSON();
        getData = JsonUtility.FromJson<OtofudaAPIJSON>(jsonText);

        var isRegistered = getData.data.registered;
        var qr = getData.data.qr;
        
        if (isRegistered)
        {
            var message = "登録情報を確認し、ボタンを押してください";
            var name = getData.data.name;
            var speed = (getData.data.hispeed / 2.0f).ToString();
            _titleSceneUiUpdater.nfcDataDisplayUi.InitUI(message, name, speed);
            
            tmpAccessCode = getData.data.public_uid;
            tmpRegistered = getData.data.registered;

            tmpHiSpeed = getData.data.hispeed / 2.0f;
            tmpName = getData.data.name;
        }
        else
        {
            var message = "登録情報を確認し、ボタンを押してください";
            var name = "未登録のユーザ";
            var speed = (getData.data.hispeed / 2.0f).ToString();
            _titleSceneUiUpdater.nfcDataDisplayUi.InitUI(message, name, speed);
            
            tmpAccessCode = getData.data.public_uid;
            tmpRegistered = getData.data.registered;

            tmpHiSpeed = 5.0f;
            tmpName = "ゲストユーザー";
            tmpQECodeURI = getData.data.qr;
        }


        
        status = TitleSceneStatus.SelectPosition;
    }
    
    
/*
    private void 
    
*/

    public void Update()
    {
        if(isFinishUserDataInput) return;
        if (playerTitleSceneInfos[0].isReady && playerTitleSceneInfos[1].isReady && status != TitleSceneStatus.StandBy)
        {
            status = TitleSceneStatus.StandBy;
            SceneManager.LoadScene("IndexJsonReadTestScene");
            isFinishUserDataInput = true;
        }
        
        if(status == TitleSceneStatus.NfcInput) return;
        if (status == TitleSceneStatus.SelectPosition)
        {
            for (int i = 0; i < playerTitleSceneInfos.Count; i++)
            {
                //準備整っちゃってたらはじく
                if(playerTitleSceneInfos[i].isReady) continue;
                
                if (playerTitleSceneInfos[i].KeyCheck())
                {
                    _titleSceneUiUpdater.nfcDataDisplayUi.parentPanel.SetActive(false);
                    _titleSceneUiUpdater.UnderMessageText.text = "";

                    _titleSceneUiUpdater.MessageUpdatePlayerUI(i, tmpAccessCode, tmpQECodeURI, tmpRegistered);
                    
                    PlayerInformationManager.Instance.hispeed[i] = tmpHiSpeed;
                    PlayerInformationManager.Instance.name[i] = tmpName;
                    status = TitleSceneStatus.WaitReady;
                    return;
                }
            }
        }
        
        if (status == TitleSceneStatus.WaitReady)
        {
            for (int i = 0; i < playerTitleSceneInfos.Count; i++)
            {
                //準備整っちゃってたらはじく
                if(playerTitleSceneInfos[i].isReady) continue;
                var index = playerTitleSceneInfos[i].KeyCheckValue();
                if (index != -1)
                {
                    //真ん中のボタンだった時
                    if (index == 2)
                    {
                        playerTitleSceneInfos[i].isReady = true;
                        status = TitleSceneStatus.NfcInput;
                        _titleSceneUiUpdater.UnderMessageText.text = underDefaultMessage;
                        return;
                    }
                }
            }
        }

    }
}
