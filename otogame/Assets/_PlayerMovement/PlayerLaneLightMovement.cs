using System.Collections;
using System.Collections.Generic;
using OtoFuda.Fumen;
using UnityEngine;

public class PlayerLaneLightMovement : PlayerMovement
{
    public override void PlayerMovementCheck(float inputMovementTime, List<NoteTimingInformation>[] timings)
    {
        base.PlayerMovementCheck(inputMovementTime, timings);

        for (var i = 0; i < PlayerKeys.Length; i++)
        {
            if (Input.GetKeyDown(PlayerKeys[i]))
            {
                _keyInputManager.keyBeamController.BeamOnAt(i);
                /*_keyInputManager.laneLight[i].SetActive(true);*/
            }

            if (Input.GetKeyUp(PlayerKeys[i]))
            {
                _keyInputManager.keyBeamController.BeamOffAt(i);
                /*_keyInputManager.laneLight[i].SetActive(false);*/
            }
        }
    }
}