using System;
using System.Collections;
using System.Collections.Generic;
using OtoFuda.RythmSystem;
using UnityEngine;

namespace OtoFuda.Fumen
{
    public class FumenFlowManager : FumenJudgeBehaviour
    {
        [Range(0, 1)] [SerializeField] private int playerID;

        public static Action<int> OnMusicStart;


        private bool isStartMusic;

        private TimingInformationList _beatLineTimingInformationList;


        //todo enveromentSetupper.csとか作ってそれにまかせる
        private void Awake()
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 60;
        }

        public override void Init(AudioSource audioSource,
            JudgeProfile profile, JudgeTextController judgeTextController,
            PlayerManager playerManager,
            TimingInformationList timingInformationList,
            List<NoteTimingInformation>[] currentStateTimingInformation,
            int[,] noteCounters)
        {
            base.Init(audioSource,
                profile, judgeTextController,
                playerManager,
                timingInformationList, currentStateTimingInformation,
                noteCounters);


            /*
            //各難易度の各レーンのノーツ情報を初期化する
            for (var i = 0; i < (int) PlayerFumenState.End; i++)
            {
                for (var k = 0; k < 5; k++)
                {
                    noteCounters[i, k] = 0;
                }
            }*/
        }

        public void SetBeatLineTimings(TimingInformationList timingInformationList)
        {
            _beatLineTimingInformationList = timingInformationList;
        }

        public void FumenUpdate()
        {
            if (!isStartMusic)
            {
                return;
            }

            for (var i = 0; i < 5; i++)
            {
                /*
                Debug.Log($"lane{i},noteCount:=>{_cacheCurrentTimings[2][i]}");
                */

                //全難易度分の譜面の通過を監視する。
                FumenPassCheck(_cacheDefaultTimings[i], PlayerFumenState.DEFAULT, i);
                FumenPassCheck(_cacheMoreDifficultTimings[i], PlayerFumenState.MORE_DIFFICULT, i);
                FumenPassCheck(_cacheMoreEasyTimings[i], PlayerFumenState.MORE_EASY, i);

                //小節線の通過を監視する
                FumenPassCheck(_beatLineTimingInformationList.DefaultTimings[i], PlayerFumenState.DefaultBeatLine, i);
                FumenPassCheck(_beatLineTimingInformationList.MoreDifficultTimings[i],
                    PlayerFumenState.MoreDifficultBeatLine, i);
                FumenPassCheck(_beatLineTimingInformationList.MoreEasyTimings[i],
                    PlayerFumenState.MoreEasyBeatLine, i);
            }
        }

        internal void StartFumenFlow(List<NoteObject> mainNotes, List<NoteObject> moreDifficultyNotes,
            List<NoteObject> defaultBeatLineNotes, List<NoteObject> moreDifficultBeatLineNotes,
            List<NoteObject> moreEasyBeatLineNotes)
        {
            var notes = mainNotes;
            var difNotes = moreDifficultyNotes;

            for (int i = 0; i < notes.Count; i++)
            {
                notes[i].ChangeFumenState();
            }

            for (int i = 0; i < difNotes.Count; i++)
            {
                difNotes[i].ChangeFumenState();
            }

            for (int i = 0; i < defaultBeatLineNotes.Count; i++)
            {
                defaultBeatLineNotes[i].ChangeFumenState();
            }

            for (int i = 0; i < moreDifficultBeatLineNotes.Count; i++)
            {
                moreDifficultBeatLineNotes[i].ChangeFumenState();
            }

            for (int i = 0; i < moreEasyBeatLineNotes.Count; i++)
            {
                moreEasyBeatLineNotes[i].ChangeFumenState();
            }


            OnMusicStart?.Invoke(playerID);
            isStartMusic = true;
        }

        private void FumenPassCheck(List<NoteTimingInformation> targetTimings,
            PlayerFumenState targetState, int index)
        {
            var stateIndex = (int) targetState;

            //targetTimingsの中身が空であればそのレーンにノーツは存在していない
            if (targetTimings.Count == 0)
            {
//                Debug.Log($"最初からないよ {index}");
                return;
            }

            //ノーツのカウントとタイミングのカウンターが一緒であればそのレーンにノーツは存在していない
            if (targetTimings.Count == _cacheNoteCounters[stateIndex, index])
            {
                //              Debug.Log($"ないよ {index}");
                return;
            }

//            Debug.Log($"note {targetTimings[noteCounters[stateIndex, index]]._noteEntity.IsActive}");

            //入力の判定がBadより負の方向へ下回っていた場合はなにもしない？
            //非アクティブだったら既に判定されている扱いなのでなにもしない
            /*if (!targetTimings[noteCounters[stateIndex, index]]._noteEntity.IsActive)
            {
                Debug.Log($"{stateIndex} の {index}番目のノーツは非アクティブです");
            }*/

            if (targetTimings[_cacheNoteCounters[stateIndex, index]]._reachTime - _audioSource.time <
                -JudgeProfile.badThreshold)
            {
                if (targetTimings[_cacheNoteCounters[stateIndex, index]]._noteType == 1 ||
                    targetTimings[_cacheNoteCounters[stateIndex, index]]._noteType == 2 ||
                    targetTimings[_cacheNoteCounters[stateIndex, index]]._noteType == 3 ||
                    targetTimings[_cacheNoteCounters[stateIndex, index]]._noteType == 4 ||
                    targetTimings[_cacheNoteCounters[stateIndex, index]]._noteType == 5)
                {
                    //引数で渡したStateが現在のプレイヤーのステートと同じであればMissの判定をする
                    //かつ、未だアクティブなノーツだった場合。(ロングなど、終点ノーツをもつやつ)
                    if (PlayerManager._players[playerID].FumenState == targetState &&
                        targetTimings[_cacheNoteCounters[stateIndex, index]]._noteEntity.IsActive)
                    {
                        var prevValue = PlayerManager._players[playerID].playerHp;
                        var currentValue = Mathf.Clamp(prevValue - 5, 0,
                            PlayerManager._players[playerID].playerMaxHp);
                        var slider = PlayerManager._players[playerID].playerHPSlider;

                        PlayerManager._players[playerID].playerHp = currentValue;
                        slider.value = currentValue;

                        //ミスしたノーツをDeactivateする。この中で終点ノーツもDeactivateされる
                        targetTimings[_cacheNoteCounters[stateIndex, index]]._noteEntity.Deactivate(Judge.Miss);
                        JudgeController.ComboCut(Judge.Miss);

                        Debug.Log("Cut");

                        //アクティブな音札ノーツをスルーしたとき、譜面をデフォルトに戻す
                        if (targetTimings[_cacheNoteCounters[stateIndex, index]]._noteType == 5)
                        {
                            if (PlayerManager._players[playerID].FumenState == targetState)
                            {
                                PlayerManager._players[playerID].FumenState = PlayerFumenState.DEFAULT;
                            }
                        }
                    }
                }


                targetTimings[_cacheNoteCounters[stateIndex, index]]._noteEntity.StopRun();
                //譜面が通過したら
                _cacheNoteCounters[stateIndex, index]++;
            }
        }
    }
}