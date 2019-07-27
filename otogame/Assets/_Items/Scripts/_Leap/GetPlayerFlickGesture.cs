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

        private float[] bufferCopy;

        private WaitForEndOfFrame _waitForEndOfFrame = new WaitForEndOfFrame();
        
        private int frameCount = 0;

        
        private void Start()
        {
            palmPositionXBuffer = new float[20];
            bufferCopy = new float[palmPositionXBuffer.Length];
            palmPositionYBuffer = new float[60];

/*
            Debug.Log("fetch = "+frameGetFetechTime);
*/
            
            
            if (_controller.Devices.Count != 2)
            {
                Debug.LogError("There is " + _controller.Devices.Count + " device");
            }

/*
            StartCoroutine(getFlickGestureCoroutine());
*/
        }

        //暫定でこれ。
        private void LateUpdate()
        {
            //providerが手を見てなかったらはじく
            //本当は手が現れた、消えたのActionでコルーチンを走らせたり止めたりしたい
            if (targetProvider.CurrentFrame.Hands.Count > 0)
            {
                var nowFramePalmPositionX = targetProvider.CurrentFrame.Hands[0].PalmPosition.x;

                //いったんバッファをコピー
                Array.Copy(palmPositionXBuffer, 0, bufferCopy, 1, 19);
                //現在のframeでの手のPosを格納
                palmPositionXBuffer[0] = nowFramePalmPositionX;
                Array.Copy(bufferCopy, 1, palmPositionXBuffer, 1, 19);
                frameCount += 1;

                if (frameCount > frameGetFetechTime)
                {
                    var beforeFramePalmPositionX = palmPositionXBuffer[frameGetFetechTime];

                    if ((beforeFramePalmPositionX > nowFramePalmPositionX) &&
                        Mathf.Abs(beforeFramePalmPositionX - nowFramePalmPositionX) > flickJudgeDistance)
                    {
                        Debug.Log("Left");
                        _playerGesture = PlayerGesture.LEFT;
                        OnGetPlayerGesture?.Invoke(playerID, _playerGesture);

                    }
                    else if ((beforeFramePalmPositionX < nowFramePalmPositionX) &&
                             Mathf.Abs(beforeFramePalmPositionX - nowFramePalmPositionX) > flickJudgeDistance)
                    {
                        Debug.Log("Right");
                        _playerGesture = PlayerGesture.RIGHT;
                        OnGetPlayerGesture?.Invoke(playerID, _playerGesture);

                    }
                    else
                    {
                        _playerGesture = PlayerGesture.NONE;
                    }

                    frameCount = 0;
                }
            }

        }
        
/*

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
                    Array.Copy(palmPositionXBuffer, 0, bufferCopy, 1, 19);
                    //現在のframeでの手のPosを格納
                    palmPositionXBuffer[0] = nowFramePalmPositionX;
                    Array.Copy(bufferCopy, 1, palmPositionXBuffer, 1, 19);
                    frameCount += 1;

                    if (frameCount > frameGetFetechTime)
                    {
                        var beforeFramePalmPositionX = palmPositionXBuffer[frameGetFetechTime];
                        
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
                        else
                        {
                            _playerGesture = PlayerGesture.NONE;
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
        }*/


        private void updateBuffer()
        {

        }


    }

}
