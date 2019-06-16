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

        private Controller _controller = new Controller();
        [Range(0,1)]
        [SerializeField] private int playerID = 0;

        [SerializeField] private int frameGetFetechTime = 20;
        [SerializeField] private float flickJudgeDistance = 30;
        
        internal PlayerGesture _playerGesture = PlayerGesture.NONE;

        public static Action<int,PlayerGesture> OnGetPlayerGesture;

        private void Start()
        {
            Debug.Log(_controller.Devices.ActiveDevice);
            Debug.Log(_controller.IsConnected);

            if (_controller.Devices.Count != 2)
            {
                Debug.LogError("There is " + _controller.Devices.Count + " device");
            }
        }

        private void Update()
        {
            var _frame = _controller.Frame();

            if (_frame.Hands.Count >= 1 && _controller.Frame(frameGetFetechTime) != null)
            {
                var nowFramePalmPositionX = _controller.Frame(0).Hands[0].PalmPosition.x;
                var beforeFramePalmPositionX = _controller.Frame(frameGetFetechTime).Hands[0].PalmPosition.x;

                var nowFramePalmPositionY = _controller.Frame(0).Hands[0].PalmPosition.y;
                var beforeFramePalmPositionY = _controller.Frame(frameGetFetechTime).Hands[0].PalmPosition.y;

                if ((beforeFramePalmPositionX > nowFramePalmPositionX) &&
                    Mathf.Abs(beforeFramePalmPositionX - nowFramePalmPositionX) > flickJudgeDistance)
                {
                    _playerGesture = PlayerGesture.LEFT;
                }
                else if ((beforeFramePalmPositionX < nowFramePalmPositionX) &&
                         Mathf.Abs(beforeFramePalmPositionX - nowFramePalmPositionX) > flickJudgeDistance)
                {
                    _playerGesture = PlayerGesture.RIGHT;
                }

/*            else if ((beforeFramePalmPositionY - nowFramePalmPositionY) < -flickJudgeDistance)
            {
                Debug.Log("UP!!!");
            }
            else if ((beforeFramePalmPositionY - nowFramePalmPositionY) > flickJudgeDistance)
            {
                Debug.Log("Down!!!");
            }*/

                else
                {
                    _playerGesture = PlayerGesture.NONE;
                }

                //アクションを発火
                OnGetPlayerGesture?.Invoke(playerID, _playerGesture);

            }

        }
    }

}
