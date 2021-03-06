﻿using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using shigeno_EditorUtility;
using UnityEditor;
using UnityEngine;

namespace OtoFuda.RythmSystem
{
    public class MusicManager : SingletonMonoBehaviour<MusicManager>
    {
        private void Start()
        {

        }
        private void Update()
        {

        }

        //サウンド名で指定する場合
        public void Startmusic(string targetSoundName)
        {
            AudioSource source = SoundManager.Instance.playSound(targetSoundName);
            source.Play();
        }

        //インデックスで指定する場合
        public void Startmusic(int targetSoundIdx)
        {
            AudioSource source = SoundManager.Instance.playSound(targetSoundIdx);
            source.Play();
        }



    }
}