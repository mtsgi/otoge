using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx.Async;
public class TitleSceneUIUpdater : MonoBehaviour
{
    public GameObject standByUIObject;
    
    public Text UnderMessageText;

    //Nfc-Apiで参照してDisplayするやつ
    public NfcDataDisplayUI nfcDataDisplayUi = new NfcDataDisplayUI();
    
    [Serializable]
    public class NfcDataDisplayUI
    {
        public GameObject parentPanel;
        
        public Text message_text;
        public Text name_text;
        public bool slowfast;
        public Text hispeed_text;
        //後日アイコンや最終プレー日時のUI追加

        public void InitUI(string message,string name,string speed)
        {
            parentPanel.SetActive(true);
            parentPanel.GetComponent<Animator>().Play("Active");
            
            message_text.text = message;
            name_text.text = name;
            hispeed_text.text = speed;
            
        }
    }
    
    public List<PlayerTitleUI> PlayerTitleUis = new List<PlayerTitleUI>();
    [Serializable]
    public class PlayerTitleUI
    {
        public GameObject playerUiPanel;
        
        public GameObject AccessCodePanel;
        public GameObject OtofudaHintPanel;

        public Text AccessCodeText;
        public Text MessageText;
    }



/*    public void ActivatePlayerUI(int playerId,string message)
    {
        var targetUIs = PlayerTitleUis[playerId]; 
        targetUIs.playerUiPanel.SetActive(true);
        targetUIs.playerUiPanel.GetComponent<Animator>().Play("Active");
        
        targetUIs.MessageText.text = message;
        
    }*/

    public void MessageUpdatePlayerUI(int playerId, string accessCode, bool isRegistered)
    {
        var targetUIs = PlayerTitleUis[playerId];
        
        targetUIs.playerUiPanel.SetActive(true);
        
        if (isRegistered)
        {
            targetUIs.AccessCodePanel.SetActive(false);
            targetUIs.OtofudaHintPanel.SetActive(true);
            
            targetUIs.AccessCodeText.text = "";
        }
        else
        {
            targetUIs.AccessCodePanel.SetActive(true);
            targetUIs.OtofudaHintPanel.SetActive(false);
            targetUIs.AccessCodeText.text = accessCode;
        }
        
        targetUIs.MessageText.text = "準備ができたら真ん中のボタンを押してください。";


    }



}


