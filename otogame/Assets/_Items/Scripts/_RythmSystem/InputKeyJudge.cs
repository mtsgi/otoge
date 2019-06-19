using System;
using System.Collections;
using System.Collections.Generic;
using OtoFuda.Fumen;
using OtoFuda.RythmSystem;
using UnityEngine;
using UnityEngine.UI;

namespace OtoFuda.player
{
    public class InputKeyJudge : MonoBehaviour
    {
        //ユーザーのキーの配列。多分10になったりする
        public KeyCode[] playerKeys = new KeyCode[5];

        //判定を表示する用のテキスト
        public Text judgeText;

        private FumenDataManager _fumenDataManager;

        private AudioSource _audioSource;

        //各レーンのノーツ数を格納する配列
        private int[] noteCount = new int[10];

        [SerializeField] private GameObject[] laneLight;
        

        //ロングノーツの開始をチェックしておくbool
        private bool[] isLongNoteStart = new bool[10];


        private void Start()
        {
            for (int i = 0; i < laneLight.Length; i++)
            {
                laneLight[i].SetActive(false);
            }

            for (int i = 0; i < isLongNoteStart.Length; i++)
            {
                isLongNoteStart[i] = false;
            }

            _audioSource = SoundManager.Instance.gameObject.GetComponents<AudioSource>()[0];
            _fumenDataManager = FumenDataManager.Instance;
        }

        private void OnEnable()
        {
            GetPlayerFlickGesture.OnGetPlayerGesture += OnGetPlayerGesture;
            GetPlayerBuddhaGesture.OnGetPlayerBuddhaGesture += OnGetPlayerBuddhaGesture;
            GetPlayerBuddhaGesture.OnReleasePlayerBuddhaPalm += OnReleasePlayerBuddhaPalm;
            CustomHandEnableDisable.OnGetPlayerHandFinish += OnGetPlayerHandFinish;
        }


        [ContextMenu("test")]
        private void test()
        {
            for (int i = 0; i < FumenDataManager.Instance.timings.Length; i++)
            {
                for (int k = 0; k < FumenDataManager.Instance.timings[i].Count; k++)
                {
                    Debug.Log(FumenDataManager.Instance.timings[i][noteCount[i]].reachTime);
                    noteCount[i]++;
                }
            }
        }


        //Updateでキー入力するより遅延が少ない、らしい(？)
        private void OnGUI()
        {

            var notesDatas = _fumenDataManager.timings;

            //推したとき        
            if (Event.current.type == EventType.KeyDown)
            {
                if (Input.GetKeyDown(Event.current.keyCode))
                {
                    for (int i = 0; i < playerKeys.Length; i++)
                    {
                        if (Event.current.keyCode == playerKeys[i])
                        {
                            checkSingleJudge(i);
                        }
                    }
                }

                return;
            }

            //離したとき
            if (Input.GetKeyUp(Event.current.keyCode))
            {
                for (int i = 0; i < playerKeys.Length; i++)
                {
                    if (Event.current.keyCode == playerKeys[i])
                    {
                        laneLight[i].SetActive(false);
                        if (isLongNoteStart[i])
                        {
                            checkSingleJudge(i);
                            isLongNoteStart[i] = false;
                            Debug.Log("<color=yellow>LongEnd</color>");
                        }
                    }
                }

                return;
            }


            //判定ラインの通過を見る
            for (int i = 0; i < _fumenDataManager.timings.Length; i++)
            {
                for (int k = 0; k < _fumenDataManager.timings[i].Count; k++)
                {
                    if (noteCount[i] != _fumenDataManager.timings[i].Count)
                    {
                        if (_audioSource.time - _fumenDataManager.timings[i][noteCount[i]].reachTime >= 0.12f)
                        {
                            judgeText.text = "MISS";
                            Debug.LogError("Miss");

                            //ロングノーツの場合、始点をミスしたら終点もミス扱いにする
                            if (_fumenDataManager.timings[i][noteCount[i]].noteType == 2)
                            {
                                //2ノーツ分カウンターを進める
                                noteCount[i] += 2;
                                judgeText.text = "MISS";
                                Debug.LogError("Miss");
                            }
                            else
                            {
                                noteCount[i]++;
                            }
                        }
                    }
                }
            }

        }



        private void checkSingleJudge(int targetLane)
        {
            laneLight[targetLane].SetActive(true);

            //押下したキーに対応するレーンに流れるすべてのノーツ情報。
            var targetLaneNoteInfos = _fumenDataManager.timings[targetLane];

            //レーンのカウント数が最大数と同じであればはじく
            if (targetLaneNoteInfos.Count == noteCount[targetLane])
            {
                return;
            }

            //現在の次に来るはずのノーツ情報
            var nextNoteTimingInfo = targetLaneNoteInfos[noteCount[targetLane]];

            //入力された時点での楽曲の再生時間と、そのレーンのノーツの到達時間の差を見る
            var inputTime = _audioSource.time;
            var judgeTime = nextNoteTimingInfo.reachTime;
            var noteType = nextNoteTimingInfo.noteType;


            //シングルノーツだった場合の判定
            //ここの判定幅要修正
            //外部からまとめて指定できるようにしたい
            if (noteType == 1 || noteType == 2)
            {
                if (isLongNoteStart[targetLane])
                {
                    checkJudgeLongNoteEnd(targetLane, inputTime, judgeTime);
                }
                else
                {
                    checkJudgeTapNote(targetLane, inputTime, judgeTime, noteType);
                }
            }
        }


        //シングルノーツの判定
        private void checkJudgeTapNote(int _targetLane, float _inputTime, float _judgeTime, int _noteType)
        {

            if (_inputTime - _judgeTime >= -0.15f && _inputTime - _judgeTime <= 0.15f)
            {
                checkLongStartNote(_targetLane, _noteType);
                noteCount[_targetLane]++;
                judgeText.text = "PERFECT";
                Debug.Log("Perfect");
            }
            else if (-0.2f <= _inputTime - _judgeTime && _inputTime - _judgeTime <= 0.2f)
            {
                checkLongStartNote(_targetLane, _noteType);
                noteCount[_targetLane]++;
                judgeText.text = "GOOD";
                Debug.Log("Good");
            }
            else if (_inputTime - _judgeTime >= -0.6f && _inputTime - _judgeTime <= 0.6f)
            {
                checkLongStartNote(_targetLane, _noteType);
                noteCount[_targetLane]++;
                judgeText.text = "BAD";
                Debug.Log("Bad");
            }
            else
            {

            }
        }

        //ロングノーツの始点を判定する関数
        private void checkLongStartNote(int _lane, int _type)
        {
            //isHaveChildNoteみたいな変数をノーツ生成時に持たせてあげることができれば、下フリックの直後に
            //ロングノーツを配置することもできるようになりそう。
            //具体的には_type==2で判定している部分をisHaveChildNote == trueであれば
            //(Json上でNoteの中にNoteが存在していれば)そのノーツはロングノーツの始点ノーツである。と判定ができる
            /*if (_fumenDataManager.timings[_lane][noteCount[_lane]].isHaveChildNote)
            {
                
            }*/
            
            if (_type == 2)
            {
                Debug.Log("<color=yellow>StartLong</color>");
                isLongNoteStart[_lane] = true;
            }
        }

        //ロングノーツの終点の判定を行う関数
        private void checkJudgeLongNoteEnd(int _targetLane, float _inputTime, float _judgeTime)
        {
            if (_inputTime - _judgeTime >= -0.15f && _inputTime - _judgeTime <= 0.15f)
            {
                noteCount[_targetLane]++;
                judgeText.text = "PERFECT";
                Debug.Log("Perfect");
            }
            else if (-0.2f <= _inputTime - _judgeTime && _inputTime - _judgeTime <= 0.2f)
            {
                noteCount[_targetLane]++;
                judgeText.text = "GOOD";
                Debug.Log("Good");
            }
            else if (_inputTime - _judgeTime >= -0.6f && _inputTime - _judgeTime <= 0.6f)
            {
                noteCount[_targetLane]++;
                judgeText.text = "BAD";
                Debug.Log("Bad");
            }
            else
            {
                noteCount[_targetLane]++;
                judgeText.text = "MISS";
                Debug.LogError("Miss");
            }

            isLongNoteStart[_targetLane] = false;
        }
        
        //フリックノーツの判定
        //フリックジェスチャによるイベント
        private void OnGetPlayerGesture(int _playerId, PlayerGesture _gesture)
        {
            if (_gesture != PlayerGesture.NONE)
            {
                Debug.Log("GetsGesture!" + ":" + _gesture);
                checkJudgeFlickNote(_gesture);
            }
        }

        private void checkJudgeFlickNote(PlayerGesture _playerGesture)
        {
            for (int i = 0; i < 5; i++)
            {
                //押下したキーに対応するレーンに流れるすべてのノーツ情報。
                var targetLaneNoteInfos = _fumenDataManager.timings[i];
                
                //レーンのカウント数が最大数と同じであればはじく
                if (targetLaneNoteInfos.Count == noteCount[i])
                { 
                    continue;
                }
                
                //現在の次に来るはずのノーツ情報
                var nextNoteTimingInfo = targetLaneNoteInfos[noteCount[i]];
                //入力された時点での楽曲の再生時間と、そのレーンのノーツの到達時間の差を見る
                var inputTime = _audioSource.time;
                var judgeTime = nextNoteTimingInfo.reachTime;
                var noteType = nextNoteTimingInfo.noteType;
                
                //左フリックだった時
                if (nextNoteTimingInfo.noteType == 3)
                {
                    //ジェスチャがLeftでなかったらBad
                    if (_playerGesture == PlayerGesture.LEFT &&
                        inputTime - judgeTime >= -0.6f && inputTime - judgeTime <= 0.6f)
                    {
                        noteCount[i]++;
                        judgeText.text = "PERFECT";
                        Debug.Log("Perfect");
                    }
                    else if (_playerGesture != PlayerGesture.LEFT 
                             && inputTime - judgeTime >= -0.6f && inputTime - judgeTime <= 0.6f)
                    {
                        noteCount[i]++;
                        judgeText.text = "BAD";
                        Debug.Log("Bad");
                    }
                    else
                    {

                    }
                }
                //右フリックだった時
                else if (nextNoteTimingInfo.noteType == 4)
                {
                    //ジェスチャがLeftでなかったらBad
                    if (_playerGesture == PlayerGesture.RIGHT &&
                        inputTime - judgeTime >= -0.6f && inputTime - judgeTime <= 0.6f)
                    {
                        noteCount[i]++;
                        judgeText.text = "PERFECT";
                        Debug.Log("Perfect");
                    }
                    else if (_playerGesture != PlayerGesture.RIGHT 
                             && inputTime - judgeTime >= -0.6f && inputTime - judgeTime <= 0.6f)
                    {
                        noteCount[i]++;
                        judgeText.text = "BAD";
                        Debug.Log("Bad");
                    }
                    else
                    {

                    }
                }
                
            }

        }

        
        //音札ノーツの判定
        private void OnGetPlayerBuddhaGesture(int _playerID)
        {
            Debug.Log("getBuddha");
            for (int i = 0; i < laneLight.Length; i++)
            {
                laneLight[i].SetActive(true);
            }
            
            checkOtoFudaJudge(2);
        }
        //手をたたいた時
        private void checkOtoFudaJudge(int targetLane)
        {
            //押下したキーに対応するレーンに流れるすべてのノーツ情報。
            var targetLaneNoteInfos = _fumenDataManager.timings[targetLane];

            //レーンのカウント数が最大数と同じであればはじく
            if (targetLaneNoteInfos.Count == noteCount[targetLane])
            {
                return;
            }

            //現在の次に来るはずのノーツ情報
            var nextNoteTimingInfo = targetLaneNoteInfos[noteCount[targetLane]];

            if (nextNoteTimingInfo.noteType != 5)
            {
                return;
            }
            
            //入力された時点での楽曲の再生時間と、そのレーンのノーツの到達時間の差を見る
            var _inputTime = _audioSource.time;
            var _judgeTime = nextNoteTimingInfo.reachTime;
            var _noteType = nextNoteTimingInfo.noteType;
            
            if (_inputTime - _judgeTime >= -0.15f && _inputTime - _judgeTime <= 0.15f)
            {
                noteCount[2]++;
                judgeText.text = "PERFECT";
                Debug.Log("Perfect");
            }
            else
            {

            }
        }
        
        //手を離したとき
        //判定ラインの光を消す処理
        private void OnReleasePlayerBuddhaPalm(int _playerID)
        {
            for (int i = 0; i < laneLight.Length; i++)
            {
                laneLight[i].SetActive(false);
            }
        }

        private void OnGetPlayerHandFinish(int _playerID)
        {
            for (int i = 0; i < laneLight.Length; i++)
            {
                laneLight[i].SetActive(false);
            }
        }
        
        
        
    }

}
