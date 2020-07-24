using System;
using System.Collections;
using System.Collections.Generic;
using OtoFuda.Fumen;
using OtoFuda.RythmSystem;
using UnityEngine;

namespace OtoFuda.Fumen
{
    public class FumenDataManagerForAutoPlay : FumenDataManager
    {
        private new void Start()
        {
            //元々のStartをかきけしたいのでこうしておく
            return;
        }
        
        public void Update()
        {
            if (Input.GetKeyUp(KeyCode.A))
            {
                StartAutoPlay();
            }
            Check();
        }
        
        public void StartAutoPlay()
        {
            MusicManager.Instance.StopMusic(0);

            foreach (Transform n in notesRootTransform.transform)
            {
                GameObject.Destroy(n.gameObject);
            }
            Init();
            FumenStart();
        }

        public override void SetAutoPlayMusicData()
        {
            base.SetAutoPlayMusicData();
            
        }
    }
}