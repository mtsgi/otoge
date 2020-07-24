using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLaneLightMovement : PlayerMovement
{
    public override void PlayerMovementCheck()
    {
        base.PlayerMovementCheck();
        for (var i = 0; i < PlayerKeys.Length; i++)
        {
            if (Input.GetKeyDown(PlayerKeys[i]))
            {
                _inputManager.keyBeamController.BeamOnAt(i);
                /*_inputManager.laneLight[i].SetActive(true);*/
            }
            
            if (Input.GetKeyUp(PlayerKeys[i]))
            {
                _inputManager.keyBeamController.BeamOffAt(i);
                /*_inputManager.laneLight[i].SetActive(false);*/
            }
        }
    }
}
