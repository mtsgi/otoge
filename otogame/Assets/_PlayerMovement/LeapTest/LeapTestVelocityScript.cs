using System.Collections;
using System.Collections.Generic;
using Leap.Unity;
using UnityEngine;

public class LeapTestVelocityScript : MonoBehaviour
{

    [SerializeField] private LeapProvider _leapProvider;
    [SerializeField] private float velocityJudgeThreshold = 0.2f;

    private void Awake()
    {
        
    }
    
    private void Update()
    {
        if (_leapProvider.CurrentFrame.Hands.Count != 0)
        {
            var vel = _leapProvider.CurrentFrame.Hands[0].PalmVelocity;
            if (vel.x > velocityJudgeThreshold)
            {
                Debug.Log("Right!!!");
            }
            else if (vel.x < -velocityJudgeThreshold)
            {
                Debug.Log("Left!!!");
            }

        }

    }
}
