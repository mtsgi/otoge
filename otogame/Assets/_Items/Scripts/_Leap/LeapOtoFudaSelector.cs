using System;
using System.Collections;
using System.Collections.Generic;
using Leap;
using Leap.Unity;
using LeapInternal;
using UnityEngine;

namespace OtoFuda.player
{
    public class LeapOtoFudaSelector : MonoBehaviour
    {
        [Range(0,1)]
        [SerializeField] private int playerID;

        [SerializeField] private float rayMaxDistance = 10.0f;
        private RaycastHit _raycastHit;
        
        private Controller _controller = new Controller();
        [SerializeField] private int frameGetFetechTime = 20;
        [SerializeField] private float fingerIndexJudgeAngle = 3;
        
        private void Update()
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
            
            var nowFrameIndexAngleY = _controller.Frame(0).Hands[0].Fingers[1].Bone(0).Rotation;
            Debug.Log(360.0f - nowFrameIndexAngleY.ToQuaternion().eulerAngles.y);
            var nowIndexAngle = 360.0f - nowFrameIndexAngleY.ToQuaternion().eulerAngles.y;
            
            var beforeFrameIndexAngleY = _controller.Frame(frameGetFetechTime).Hands[0].Fingers[1].Bone(0).Rotation;
            Debug.Log(360.0f - beforeFrameIndexAngleY.ToQuaternion().eulerAngles.y);
            var beforeIndexAngle = 360.0f - beforeFrameIndexAngleY.ToQuaternion().eulerAngles.y;

            var indexAngles = Mathf.Abs(nowIndexAngle - beforeIndexAngle);
            Debug.Log(indexAngles);
            if (indexAngles > fingerIndexJudgeAngle)
            {
                Debug.Log("</color=blue> OVER!!!!! </color>");
            }
            


            var nowFramePalmPositionY = _controller.Frame(0).Hands[0].PalmPosition.y;
            var beforeFramePalmPositionY = _controller.Frame(frameGetFetechTime).Hands[0].PalmPosition.y;
            
            
        }
    }

}

