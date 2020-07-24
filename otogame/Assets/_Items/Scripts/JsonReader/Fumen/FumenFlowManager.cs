using System;
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

        public static Action<int> OnMusicStart; 

        private void Awake()
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 60;
        }

        private void Start()
        {
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {


            }
            
        }

        internal void StartFumenFlow()
        {
            var notes = FumenDataManager.Instance.mainNotes[playerID];
            var difNotes = FumenDataManager.Instance.moreDifficultNotes[playerID];

            for (int i = 0; i < notes.Count; i++)
            {
                notes[i].ChangeFumenState();
            }

            for (int i = 0; i < difNotes.Count; i++)
            {
                difNotes[i].ChangeFumenState();
            }
                
                
            OnMusicStart?.Invoke(playerID);
        }



        
        
        

        
    }
}
