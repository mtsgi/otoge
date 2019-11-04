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
        public OtofudaSelectorProfile SelectorProfile;
        private float judgeWidth;
        private float judgeInterval;

        private float centerMid;
        private float left1Mid;
        private float left2Mid;
        private float right1Mid;
        private float right2Mid;



        [Range(0, 1)] [SerializeField] private int playerID = 0;
        [SerializeField] private float judgeRate = 15;

        [SerializeField] private LeapProvider _provider;

        private PlayerSelectState _selectState = PlayerSelectState.CENTER;

        //一指し指の指す方向が変更されたときに発火するアクション
        public static Action<int, PlayerSelectState> OnPlayerFocusCardChange;
        
        public static Action<int, PlayerSelectState> OnPlayerSelectCardChange;

        private void Start()
        {
            judgeInterval = SelectorProfile.judgeInterval;
            judgeWidth = SelectorProfile.judgeWidth;

            centerMid = 0.0f;
            left1Mid = judgeInterval * -1;
            left2Mid = judgeInterval * -2;
            right1Mid = judgeInterval * 1;
            right2Mid = judgeInterval * 2;

/*            Debug.Log((left2Mid + (judgeWidth / 2)));

            Debug.Log(((left1Mid - (judgeWidth / 2))));
            Debug.Log((left1Mid + (judgeWidth / 2)));

            Debug.Log((centerMid - (judgeWidth / 2)));
            Debug.Log((centerMid + (judgeWidth / 2)));

            Debug.Log((right1Mid - (judgeWidth / 2)));
            Debug.Log((right1Mid + (judgeWidth / 2)));

            Debug.Log((right2Mid - (judgeWidth / 2)));*/

        }

        private void Update()
        {
            if (_provider.CurrentFrame.Hands.Count == 0)
            {
                return;
            }
            var fingerPosX = _provider.CurrentFrame.Hands[0].Fingers[1].TipPosition.x;
            
            if (_selectState != PlayerSelectState.CENTER
                   && centerMid - (judgeWidth/2) < fingerPosX && fingerPosX < centerMid + (judgeWidth/2))
            {
                _selectState = PlayerSelectState.CENTER;
                OnPlayerFocusCardChange?.Invoke(playerID,_selectState);

/*                Debug.Log("Center");*/

            }
            else if (_selectState != PlayerSelectState.RIGHT1
                     && right1Mid - (judgeWidth/2) <= fingerPosX && fingerPosX < right1Mid + (judgeWidth/2))
            {
                _selectState = PlayerSelectState.RIGHT1;
                OnPlayerFocusCardChange?.Invoke(playerID,_selectState);

/*                Debug.Log("Right1");*/

            }
            else if (_selectState != PlayerSelectState.RIGHT2
                     && right2Mid - (judgeWidth/2) <= fingerPosX)
            {
                _selectState = PlayerSelectState.RIGHT2;
                OnPlayerFocusCardChange?.Invoke(playerID,_selectState);

/*                Debug.Log("Right2");*/

            }
            else if (_selectState != PlayerSelectState.LEFT1
                     && left1Mid - (judgeWidth / 2) < fingerPosX && fingerPosX <= left1Mid + (judgeWidth / 2))
            {
                _selectState = PlayerSelectState.LEFT1;
                OnPlayerFocusCardChange?.Invoke(playerID, _selectState);

/*                Debug.Log("Left1");*/

            }
            else if (_selectState != PlayerSelectState.LEFT2
                     && fingerPosX <= left2Mid + (judgeWidth/2))
            {
                _selectState = PlayerSelectState.LEFT2;
                OnPlayerFocusCardChange?.Invoke(playerID,_selectState);

/*                Debug.Log("Left2");*/

            }
            else
            {
                
            }

            var indexTipHeight = _provider.CurrentFrame.Hands[0].Fingers[1].TipPosition.y;
            
            var middleTipHeight = _provider.CurrentFrame.Hands[0].Fingers[2].TipPosition.y;
            var littleTipHeight = _provider.CurrentFrame.Hands[0].Fingers[3].TipPosition.y;
            var ringTipHeight = _provider.CurrentFrame.Hands[0].Fingers[4].TipPosition.y;

            var fingerHeightAverage = (middleTipHeight + littleTipHeight + ringTipHeight) / 3;
            
/*
            Debug.Log(fingerHeightAverage+"___"+indexTipHeight);
*/


            //押下ジェスチャの取得
            if (fingerHeightAverage - indexTipHeight > judgeRate)
            {
                //発火
                OnPlayerSelectCardChange?.Invoke(playerID, _selectState);
            }
        }
    }

}

