using System;
using System.Collections;
using System.Collections.Generic;
using Leap;
using Leap.Unity;
using LeapInternal;
using UnityEngine;

namespace OtoFuda.player
{
    public enum PlayerSelectState
    {
        LEFT2,
        LEFT1,
        CENTER,
        RIGHT1,
        RIGHT2,
    }
    
    public class LeapOtoFudaSelector : MonoBehaviour
    {
        [Range(0,1)]
        [SerializeField] private int playerID;
        
        private Controller _controller = new Controller();

        private PlayerSelectState _selectState = PlayerSelectState.CENTER;

        //一指し指の指す方向が変更されたときに発火するアクション
        public static Action<int, PlayerSelectState> OnPlayerSelectCardChange;
        
        private void Update()
        {
            var fingerPosX = _controller.Frame(0).Hands[0].Fingers[1].TipPosition.x;

            if (_selectState != PlayerSelectState.CENTER
                   && -30 < fingerPosX && fingerPosX < 30)
            {
                _selectState = PlayerSelectState.CENTER;
                OnPlayerSelectCardChange?.Invoke(playerID,_selectState);
                Debug.Log("Center");
            }
            else if (_selectState != PlayerSelectState.RIGHT1
                     && 50 <= fingerPosX && fingerPosX < 110)
            {
                _selectState = PlayerSelectState.RIGHT1;
                OnPlayerSelectCardChange?.Invoke(playerID,_selectState);
                Debug.Log("Right1");
            }
            else if (_selectState != PlayerSelectState.RIGHT2
                     && 130 <= fingerPosX)
            {
                _selectState = PlayerSelectState.RIGHT2;
                OnPlayerSelectCardChange?.Invoke(playerID,_selectState);
                Debug.Log("Right2");
            }
            else if (_selectState != PlayerSelectState.LEFT1
                     && -110 < fingerPosX && fingerPosX <= -50)
            {
                _selectState = PlayerSelectState.LEFT1;
                OnPlayerSelectCardChange?.Invoke(playerID,_selectState);
                Debug.Log("Left1");
            }
            else if (_selectState != PlayerSelectState.LEFT2
                     && fingerPosX <= -130)
            {
                _selectState = PlayerSelectState.LEFT2;
                OnPlayerSelectCardChange?.Invoke(playerID,_selectState);
                Debug.Log("Left2");
            }
            else
            {
                
            }
            
            
            
        }
    }

}

