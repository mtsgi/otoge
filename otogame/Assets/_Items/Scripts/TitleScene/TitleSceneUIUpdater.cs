using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleSceneUIUpdater : MonoBehaviour
{
    public GameObject standByUIObject;
    
    public Text MainMessageText;
    public Text UnderMessageText;
    
    public List<PlayerTitleUI> PlayerTitleUis = new List<PlayerTitleUI>();
    [Serializable]
    public class PlayerTitleUI
    {
        public GameObject playerUiPanel;
        public Text MessageText;
    }



    public void ActivatePlayerUI(int playerId)
    {
        var targetUIs =PlayerTitleUis[playerId]; 
        targetUIs.playerUiPanel.SetActive(true);
        targetUIs.playerUiPanel.GetComponent<Animator>().Play("Active");
    }
    
    public void MessageUpdatePlayerUI(int playerId,string message)
    {
        var targetUIs =PlayerTitleUis[playerId];
        targetUIs.MessageText.text = message;
    }
    
}


