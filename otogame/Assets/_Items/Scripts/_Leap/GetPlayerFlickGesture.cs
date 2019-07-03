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

        [SerializeField] private int frameGetFetechTime = 20;
        [SerializeField] private float flickJudgeDistance = 30;
        
        private PlayerGesture _playerGesture = PlayerGesture.NONE;

        public static Action<int,PlayerGesture> OnGetPlayerGesture;

        private float[] palmPositionXBuffer;
        private float[] palmPositionYBuffer;

        private WaitForEndOfFrame _waitForEndOfFrame = new WaitForEndOfFrame();
        
        private UInt32 frameCount = 0;

        
        private void Start()
        {
            palmPositionXBuffer = new float[60];
            palmPositionYBuffer = new float[60];

            Debug.Log("buffer size = "+frameGetFetechTime);
            
            
            if (_controller.Devices.Count != 2)
            {
                Debug.LogError("There is " + _controller.Devices.Count + " device");
            }

            StartCoroutine(getFlickGestureCoroutine());
        }

        private void Update()
        {
            return;
            var _frame = _controller.Frame();


            if (_frame.Hands.Count >= 1 )
            {
                if (Connection.GetConnection().Frames.Count < 30 &&
                    _controller.Frame(frameGetFetechTime).Hands.Count < 30)
                {
                    return;
                }

                if (_controller.Frame(frameGetFetechTime).Hands.Count == 0)
                {
                    return;
                }

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


        private IEnumerator getFlickGestureCoroutine()
        {
            var bufferCopy = new float[60];
            while (true)
            {
                //providerが手を見てなかったらはじく
                //本当は手が現れた、消えたのActionでコルーチンを走らせたり止めたりしたい
                if (targetProvider.CurrentFrame.Hands.Count > 0)
                {
                    var nowFramePalmPositionX = targetProvider.CurrentFrame.Hands[0].PalmPosition.x;

                    //いったんバッファをコピー
                    Array.Copy(palmPositionXBuffer, 0, bufferCopy, 1, 59);
                    //現在のframeでの手のPosを格納
                    palmPositionXBuffer[0] = nowFramePalmPositionX;
                    Array.Copy(bufferCopy, 1, palmPositionXBuffer, 1, 59);
                    
                    frameCount += 1;

                    if (frameCount > frameGetFetechTime)
                    {
                        var beforeFramePalmPositionX = palmPositionXBuffer[frameGetFetechTime];
                        //Debug.Log(Mathf.Abs(beforeFramePalmPositionX - nowFramePalmPositionX));
                        
                        if ((beforeFramePalmPositionX > nowFramePalmPositionX) &&
                            Mathf.Abs(beforeFramePalmPositionX - nowFramePalmPositionX) > flickJudgeDistance)
                        {
                            _playerGesture = PlayerGesture.LEFT;
                            //Debug.Log("Player"+(playerID+1)+" left");
                        }
                        else if ((beforeFramePalmPositionX < nowFramePalmPositionX) &&
                                 Mathf.Abs(beforeFramePalmPositionX - nowFramePalmPositionX) > flickJudgeDistance)
                        {
                            _playerGesture = PlayerGesture.RIGHT;
                            //Debug.Log("Player"+(playerID+1)+" right");

                        }
                        else
                        {
                            _playerGesture = PlayerGesture.NONE;
                         //   Debug.Log("NOne");
                        }
                        
                        OnGetPlayerGesture?.Invoke(playerID, _playerGesture);

                    }
                    
                    yield return _waitForEndOfFrame;
                }
                else
                {
                    yield return null;
                }
            }
        }


        private void updateBuffer()
        {

        }


    }

}
