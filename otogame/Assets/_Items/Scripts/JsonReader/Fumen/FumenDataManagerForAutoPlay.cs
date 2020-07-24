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
        [SerializeField] private AutoPlaySettingInputter _autoPlaySettingInputter;

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
            FumenStart(musicDataMode);
        }

        public override void SetAutoPlayMusicData()
        {
            base.SetAutoPlayMusicData();
            //テスト
            //SetDebugMusicData();
            //

            var autoSetting = _autoPlaySettingInputter.GetAutoPlaySetting();

            _musicData.jsonFilePath = autoSetting.jsonFilePath;

            _musicData.bpm = autoSetting.bpm;
            _musicData.beat = autoSetting.beat;
            _musicData.offset = autoSetting.offset;

            _musicData.levels = autoSetting.levels;
            _musicData.musicId = autoSetting.musicFilePath;
        }
    }
}