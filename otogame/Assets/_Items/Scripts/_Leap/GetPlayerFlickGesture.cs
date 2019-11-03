using System;
using System.Collections;
using System.Collections.Generic;
using Leap;
using Leap.Unity;
using LeapInternal;
using UnityEngine;

namespace OtoFuda.player
{
    //Playerの手の動きを管理
    public enum PlayerGesture
    {
        NONE,
        UP,
        DOWN,
        LEFT,
        RIGHT
    }
    
    
    public class GetPlayerFlickGesture : MonoBehaviour
    {
        [SerializeField] private LeapProvider targetProvider; 
        
        private Controller _controller = new Controller();
        [Range(0,1)]
        [SerializeField] private int playerID = 0;


        public static Action<int,PlayerGesture> OnGetPlayerGesture;
        
        
        public static PlayerGesture _playerGesture = PlayerGesture.NONE;
        [SerializeField] private float velocityJudgeThreshold = 0.44f;

        
        private void OnEnable()
        {
            if (_controller.Devices.Count != 2)
            {
                Debug.LogError("There is " + _controller.Devices.Count + " device");
            }
        }

        //暫定でこれ。
        private void Update()
        {
            if (targetProvider.CurrentFrame.Hands.Count > 0)
            {
                var vel = targetProvider.CurrentFrame.Hands[0].PalmVelocity;
//                Debug.Log(vel.x);
                if (vel.x > velocityJudgeThreshold)
                {
//                    Debug.Log("Right!!!");
                    _playerGesture = PlayerGesture.RIGHT;
                    OnGetPlayerGesture?.Invoke(playerID, _playerGesture);
                }
                else if (vel.x < -velocityJudgeThreshold)
                {
//                    Debug.Log("Left!!!");
                    _playerGesture = PlayerGesture.LEFT;
                    OnGetPlayerGesture?.Invoke(playerID, _playerGesture);
                } //上下は今は使わないよ
/*                else if (vel.y < -velocityJudgeThreshold)
                {
                    Debug.Log("Up!!!");
                    _playerGesture = PlayerGesture.UP;
                    OnGetPlayerGesture?.Invoke(playerID, _playerGesture);
                }
                else if (vel.y < -velocityJudgeThreshold)
                {
                    Debug.Log("Down!!!");
                    _playerGesture = PlayerGesture.DOWN;
                    OnGetPlayerGesture?.Invoke(playerID, _playerGesture);
                }*/
                else
                {
                    _playerGesture = PlayerGesture.NONE;
                }
                
                
            }

        }

    }

}
