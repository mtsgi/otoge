using System;
using System.Collections;
using System.Collections.Generic;
using Leap;
using Leap.Unity;
using LeapInternal;
using UnityEngine;

public class GetPlayerGesture : SingletonMonoBehaviour<GetPlayerGesture> {
    
    private Controller _controller = new Controller();
    [SerializeField] private int frameGetFetechTime = 20;
    [SerializeField] private float flickJudgeDistance = 30;
    
    public enum PlayerGesture
    {
        NONE,
        UP,
        DOWN,
        LEFT,
        RIGHT
    }

    internal PlayerGesture _playerGesture = PlayerGesture.NONE;

    private void Start()
    {
        Debug.Log(_controller.Devices.ActiveDevice);
        Debug.Log(_controller.IsConnected);
        
        if (_controller.Devices.Count != 2)
        {
            Debug.LogError("There is "+_controller.Devices.Count+" device");
        }
    }

    private void Update()
    {
        var _frame = _controller.Frame();

        if (_frame.Hands.Count >= 1 && _controller.Frame(20) != null)
        {
            var nowFramePalmPositionX = _controller.Frame(0).Hands[0].PalmPosition.x;
            var beforeFramePalmPositionX = _controller.Frame(frameGetFetechTime).Hands[0].PalmPosition.x;
            
            var nowFramePalmPositionY = _controller.Frame(0).Hands[0].PalmPosition.y;
            var beforeFramePalmPositionY = _controller.Frame(frameGetFetechTime).Hands[0].PalmPosition.y;

            if ((beforeFramePalmPositionX - nowFramePalmPositionX) < -flickJudgeDistance)
            {
                _playerGesture = PlayerGesture.RIGHT;
                Debug.Log("Right!");
            }
            else if ((beforeFramePalmPositionX - nowFramePalmPositionX) > flickJudgeDistance)
            {
                _playerGesture = PlayerGesture.LEFT;
                Debug.Log("Left!");
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
                Debug.Log("None");
            }

        }

    }
}
