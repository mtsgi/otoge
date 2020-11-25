﻿using System;
using System.Collections;
using System.Collections.Generic;
using OtoFuda.Fumen;
using OtoFuda.RythmSystem;
using UnityEngine;

namespace OtoFuda.Fumen
{
    public class FumenDataManagerForAutoPlay : FumenDataManager
    {
        [SerializeField] private AutoPlaySettingInputter autoPlaySettingInputter;

        private new void Awake()
        {
            return;
        }

        private new void Start()
        {
            base.Start();
            //元々のStartをかきけしたいのでこうしておく
            return;
        }

        public new void Update()
        {
            if (Input.GetKeyUp(KeyCode.Space))
            {
                StartAutoPlay();
                autoPlaySettingInputter.OnStartAutoPlay();
            }

            if (Input.GetKeyUp(KeyCode.Escape))
            {
                StopAutoPlay();
                autoPlaySettingInputter.OnStopAutoPlay();
            }

            base.Update();
        }

        public void StartAutoPlay()
        {
            autoPlaySettingInputter.SaveAutoPlaySetting();

            MusicManager.Instance.StopMusic(0);

            foreach (Transform n in notesRootTransform.transform)
            {
                GameObject.Destroy(n.gameObject);
            }

            Init();
            FumenStart(musicDataMode);
        }

        private void StopAutoPlay()
        {
            //終了時にコンボリセット
            for (int i = 0; i < PlayerKeyInputManagers.Length; i++)
            {
                PlayerKeyInputManagers[i].ComboCut(Judge.None);
            }

            MusicManager.Instance.StopMusic(0);

            foreach (Transform n in notesRootTransform.transform)
            {
                GameObject.Destroy(n.gameObject);
            }
        }

        public override void SetAutoPlayMusicData()
        {
            base.SetAutoPlayMusicData();

            var autoSetting = autoPlaySettingInputter.GetAutoPlaySetting();

            _musicData.jsonFilePath = autoSetting.jsonFilePath;

            _musicData.bpm = autoSetting.bpm;
            _musicData.beat = autoSetting.beat;
            _highSpeed = autoSetting.highSpeed;
            Debug.Log(_highSpeed[0]);
            _musicData.offset = autoSetting.offset;

            _musicData.levels = autoSetting.levels;
            _musicData.musicId = autoSetting.musicFilePath;
        }
    }
}