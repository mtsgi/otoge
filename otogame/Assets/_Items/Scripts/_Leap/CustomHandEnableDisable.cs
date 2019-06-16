using System;
using System.Collections;
using System.Collections.Generic;
using Leap.Unity;
using UnityEngine;

namespace OtoFuda.player
{
    public class CustomHandEnableDisable : HandEnableDisable
    {
        [Range(0,1)]
        [SerializeField] private int playerID;

        public static Action<int> OnGetPlayerHandFinish;
    
        protected override void HandFinish() 
        {
            Debug.Log("handDisable");
            //アクションを発火
            OnGetPlayerHandFinish?.Invoke(playerID);
            
            base.HandFinish();
        }
    }
}
