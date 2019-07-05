using System.Collections;
using System.Collections.Generic;
using OtoFuda.RythmSystem;
using UnityEngine;

namespace OtoFuda.Fumen
{
    public class FumenFlowManager : MonoBehaviour
    {
        [Range(0,1)]
        [SerializeField] private int playerID;

        private void Awake()
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 60;
        }

        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                var notes = FumenDataManager.Instance.mainNotes[playerID];
                for (int i = 0; i < notes.Count; i++)
                {
                    notes[i].changeFumenState();
                }
                MusicManager.Instance.Startmusic(0);

            }
        }

        
    }
}
