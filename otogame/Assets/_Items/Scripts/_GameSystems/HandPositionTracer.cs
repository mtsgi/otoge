using System;
using System.Collections;
using System.Collections.Generic;
using Leap;
using Leap.Unity;
using UnityEngine;

public class HandPositionTracer : MonoBehaviour
{
    [SerializeField] private LeapProvider provider;
    
    private Controller _controller = new Controller();
    private Vector3 defPos = new Vector3();
    private void Start()
    {

    }

    private void OnEnable()
    {
/*        if (_controller.Devices.Count == 0)
        {
            Debug.LogError("LeapMotionが一台も接続されていませんよ！");
        }*/

        defPos = gameObject.transform.position;
    }

    private void LateUpdate()
    {
        //Debug.Log(provider.CurrentFrame.Hands[0].Fingers[1].TipPosition);

        if (provider.CurrentFrame.Hands.Count > 0)
        {
            var fingerPos = provider.CurrentFrame.Hands[0].Fingers[1].TipPosition;
            var linePos = fingerPos.ToVector3().normalized;
            linePos.y = defPos.y;
            linePos.z = defPos.z;
            gameObject.transform.position = linePos;
        }
    }
}
