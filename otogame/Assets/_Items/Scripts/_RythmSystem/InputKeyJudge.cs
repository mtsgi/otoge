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
        private bool isStartMusic = false;
        
        [Range(0,1)]
        [SerializeField] private int playerID;
        public KeyCode[] playerKeys = new KeyCode[5];

        //判定を表示する用のテキスト
        [SerializeField] private Animator[] judgeTextAnimators;

        //判定をenumで管理
        private enum Judge
        {
            PERFECT = 0,
            GOOD = 1,
            BAD = 2,
            MISS = 3,
        }
        
        private FumenDataManager _fumenDataManager;

        private AudioSource _audioSource;

        private int[,] _noteCounters;
            //各レーンのノーツ数を格納する配列
/*
        private int[] noteCount = new int[5];
        private int[] moreDifficulNoteCount = new int[5];
        private int[] moreEasyNoteCount = new int[5];
*/


        [SerializeField] private GameObject[] laneLight;
        

        //ロングノーツの開始をチェックしておくbool
        private bool[] isLongNoteStart = new bool[5];
        
        //カードを使うときのアクション
        public static Action<int> OnUseOtoFudaCard;

        private PlayerManager _playerManager;



        private void Start()
        {

            _playerManager = PlayerManager.Instance;

            _noteCounters = _playerManager._players[playerID].noteCounters;
            
            for (int i = 0; i < laneLight.Length; i++)
            {
                laneLight[i].SetActive(false);
            }

            for (int i = 0; i < isLongNoteStart.Length; i++)
            {
                isLongNoteStart[i] = false;
            }

            for (int i = 0; i < 3; i++)
            {
                for (int k = 0; k < 5; k++)
                {
                    _noteCounters[i, k] = 0;
                }
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
            FumenFlowManager.OnMusicStart += OnMusicStart;
        }


        private void OnMusicStart(int _playerID)
        {
            if (_playerID == playerID)
            {
                isStartMusic = true;
            }
        }


/*
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
*/


        //Updateでキー入力するより遅延が少ない、らしい(？)
        private void OnGUI()
        {

/*
            var notesDatas = _fumenDataManager.timings;
*/

            
            //推したとき        
            if (Event.current.type == EventType.KeyDown)
            {
                if (Input.GetKeyDown(Event.current.keyCode))
                {
                    for (int i = 0; i < playerKeys.Length; i++)
                    {
                        if (Event.current.keyCode == playerKeys[i])
                        {
                            //裏で流しておく現在とは違う難易度の譜面については判定処理をスルーする。
                            switch (_playerManager._players[playerID].FumenState)
                            {
                                case PlayerFumenState.DEFAULT:
                                    checkSingleJudge(i, _fumenDataManager.timings[playerID, i], PlayerFumenState.DEFAULT);
                                    break;
                                case PlayerFumenState.MORE_DIFFICULT:
                                    checkSingleJudge(i, _fumenDataManager.moreDifficultTimings[playerID, i], PlayerFumenState.MORE_DIFFICULT);
                                    break;
                                default:
                                    break;
                            }
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

                        if (!isStartMusic)
                        {
                            return;
                        }
                        if (isLongNoteStart[i])
                        {
                            //裏で流しておく現在とは違う難易度の譜面については判定処理をスルーする。
                            switch (_playerManager._players[playerID].FumenState)
                            {
                                case PlayerFumenState.DEFAULT:
                                    checkSingleJudge(i, _fumenDataManager.timings[playerID, i], PlayerFumenState.DEFAULT);
                                    break;
                                case PlayerFumenState.MORE_DIFFICULT:
                                    checkSingleJudge(i, _fumenDataManager.moreDifficultTimings[playerID, i], PlayerFumenState.MORE_DIFFICULT);
                                    break;
                                default:
                                    break;
                            }                          
                            isLongNoteStart[i] = false;
                            Debug.Log("<color=yellow>LongEnd</color>");
                        }
                    }
                }

                return;
            }


            //判定ラインの通過を見る
            for (int i = 0; i < 5; i++)
            {
/*                var debugStr = "";
                var st ="";
                for (int j = 0; j < 2; j++)
                {
                    for (int k = 0; k < 5; k++)
                    {
                        st += "["+noteCounters[j,k]+"]";
                    }

                    debugStr += j+":"+st + " " ;
                }*/

                //通過時は各難易度ごとに譜面情報を更新しておいて、譜面難易度に変化があった時にちゃんと描画されるようにする
                fumenPassChecker(_fumenDataManager.timings[playerID,i],PlayerFumenState.DEFAULT,i);
                fumenPassChecker(_fumenDataManager.moreDifficultTimings[playerID,i],PlayerFumenState.MORE_DIFFICULT,i);

/*                Debug.Log("i"+i);
                Debug.Log( _fumenDataManager.timings[playerID,i].Count);*/



/*                for (int k = 0; k < _fumenDataManager.timings[playerID,i].Count; k++)
                {
                    if (noteCount[i] != _fumenDataManager.timings[playerID,i].Count)
                    {
                        if (_audioSource.time - _fumenDataManager.timings[playerID,i][noteCount[i]].reachTime >= 0.12f)
                        {
                            judgeTextAnimators[(int) Judge.MISS].Play("Judge", 0, 0.0f);
//                            Debug.LogError("Miss"+(int) Judge.MISS);

                            //ロングノーツの場合、始点をミスしたら終点もミス扱いにする
                            if (_fumenDataManager.timings[playerID, i][noteCount[i]].noteType == 2)
                            {
                                //2ノーツ分カウンターを進める
                                noteCount[i] += 2;
                                if (_playerManager._players[playerID].FumenState == PlayerFumenState.DEFAULT)
                                {
                                    judgeTextAnimators[(int) Judge.MISS].Play("Judge", 0, 0.0f);
                                    Debug.LogError("Miss");
                                }

                            }
                            else
                            {
                                noteCount[i]++;
                            }
                            
                            
                            //エラー回避、2レーン目のノーツ数が最大値だったらreturn
                            if (noteCount[2] == _fumenDataManager.timings[playerID, 2].Count)
                            {
                                return;
                            }
                            
                            
                            //次のノーツが音札ノーツであればターンチェックのコルーチンを走らせ始める
                            if (_fumenDataManager.timings[playerID, 2][noteCount[2]].noteType == 5)
                            {
                                _playerManager.runCoroutine();
                            }
                        }
                    }
                }*/
            }

        }
        //メモ
        //timingとState周りをリファクタ
        //ノーツが判定ラインを通過したときの処理
        private void fumenPassChecker(List<FumenDataManager.NoteTimingInfomation> targetTimings,
            PlayerFumenState targetState, int index)
        {
            var stateIndex = (int) targetState;
         
            //もうリストがなくなりきってたらはじく
            if ((stateIndex == 1 && _fumenDataManager.mainNotes[playerID].Count == 0) || 
                (stateIndex == 2 && _fumenDataManager.moreDifficultNotes[playerID].Count == 0)) 
            {
                return;
            }

            for (int k = 0; k < targetTimings.Count; k++)
            {
                if (_noteCounters[stateIndex, index] != targetTimings.Count)
                {
                    if (_audioSource.time - targetTimings[_noteCounters[stateIndex, index]].reachTime >=
                        0.12f)
                    {

                        if (_playerManager._players[playerID].FumenState == targetState)
                        {
                            judgeTextAnimators[(int) Judge.MISS].Play("Judge", 0, 0.0f);
                            Debug.LogError("Miss"+(int) Judge.MISS);
                        }
                        
                        //ロングノーツの場合、始点をミスしたら終点もミス扱いにする
                        if (targetTimings[_noteCounters[stateIndex, index]].noteType == 2)
                        {
                            //2ノーツ分カウンターを進める
                            _noteCounters[stateIndex, index] += 2;
                            _playerManager._players[playerID].noteSimpleCount += 2;
                            
                            
                            //通過に応じてRemove
                            if (stateIndex == 1)
                            {
                                _fumenDataManager.mainNotes[playerID].RemoveAt(0);
                                _fumenDataManager.mainNotes[playerID].RemoveAt(0); 
                            }
                            else if (stateIndex == 2)
                            {
                                _fumenDataManager.moreDifficultNotes[playerID].RemoveAt(0);
                                _fumenDataManager.moreDifficultNotes[playerID].RemoveAt(0);
                            }
                            
                            if (_playerManager._players[playerID].FumenState == targetState)
                            {
                                judgeTextAnimators[(int) Judge.MISS].Play("Judge", 0, 0.0f);
                                Debug.LogError("Miss");
                            }

                        }
                        else
                        {
                            _noteCounters[stateIndex, index]++;
                            _playerManager._players[playerID].noteSimpleCount++;
                            
                            //通過に応じてRemove
                            if (stateIndex == 1)
                            {
                                _fumenDataManager.mainNotes[playerID].RemoveAt(0);
                            }
                            else if (stateIndex == 2)
                            {
                                _fumenDataManager.moreDifficultNotes[playerID].RemoveAt(0);
                            }
                        }
                        
                        //エラー回避、2レーン目のノーツ数が最大値だったらreturn
                        Debug.LogWarning(_noteCounters[stateIndex, 2]);

                        if (_noteCounters[stateIndex, 2] >= targetTimings.Count)
                        {
                            return;
                        }

                        //次のノーツが音札ノーツであればターンチェックのコルーチンを走らせ始める
                        if (targetTimings[_noteCounters[stateIndex, 2]].noteType == 5)
                        {
                            _playerManager.runCoroutine();
                        }
                    }
                }
            }
            
        }



        private void checkSingleJudge(int targetLane, List<FumenDataManager.NoteTimingInfomation> targetTimings,
            PlayerFumenState fumenState)
        {
            var stateIndex = (int) fumenState;
            laneLight[targetLane].SetActive(true);

            //もうリストがなくなりきってたらはじく
            if ((stateIndex == 1 && _fumenDataManager.mainNotes[playerID].Count == 0) || 
                (stateIndex == 2 && _fumenDataManager.moreDifficultNotes[playerID].Count == 0)) 
            {
//                Debug.Log("<color=red>none");
                return;
            }
            

            if (!isStartMusic)
            {
                return;
            }

            //押下したキーに対応するレーンに流れるすべてのノーツ情報。
            var targetLaneNoteInfos = targetTimings;

            //レーンのカウント数が最大数と同じであればはじく
            if (targetLaneNoteInfos.Count <= _noteCounters[stateIndex,targetLane])
            {
                return;
            }

            //現在の次に来るはずのノーツ情報
            var nextNoteTimingInfo = targetLaneNoteInfos[_noteCounters[stateIndex,targetLane]];

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
                    checkJudgeLongNoteEnd(targetLane, inputTime, judgeTime, stateIndex);
                }
                else
                {
                    checkJudgeTapNote(targetLane, inputTime, judgeTime, noteType, stateIndex);
                }
            }
        }


        //シングルノーツの判定
        private void checkJudgeTapNote(int _targetLane, float _inputTime, float _judgeTime, int _noteType,
            int _stateIndex)
        {

            if (_inputTime - _judgeTime >= -0.15f && _inputTime - _judgeTime <= 0.15f)
            {
                checkLongStartNote(_targetLane, _noteType);
                _noteCounters[_stateIndex, _targetLane]++;
                _playerManager._players[playerID].noteSimpleCount++;
                
                judgeTextAnimators[(int) Judge.PERFECT].Play("Judge", 0, 0.0f);
                Debug.Log("Perfect");
            }
            else if (-0.2f <= _inputTime - _judgeTime && _inputTime - _judgeTime <= 0.2f)
            {
                checkLongStartNote(_targetLane, _noteType);
                _noteCounters[_stateIndex, _targetLane]++;
                _playerManager._players[playerID].noteSimpleCount++;
                judgeTextAnimators[(int) Judge.GOOD].Play("Judge", 0, 0.0f);
                Debug.Log("Good");
            }
            else if (_inputTime - _judgeTime >= -0.6f && _inputTime - _judgeTime <= 0.6f)
            {
                checkLongStartNote(_targetLane, _noteType);
                _noteCounters[_stateIndex, _targetLane]++;
                _playerManager._players[playerID].noteSimpleCount++;
                judgeTextAnimators[(int) Judge.BAD].Play("Judge", 0, 0.0f);
                Debug.Log("Bad");
            }
            else
            {

            }
            
            //判定したときは現在のステートのNoteObjectをRemoveする
            if (_stateIndex == 1)
            {
                _fumenDataManager.mainNotes[playerID].RemoveAt(0);
            }
            else if (_stateIndex == 2)
            {
                _fumenDataManager.moreDifficultNotes[playerID].RemoveAt(0);
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
        private void checkJudgeLongNoteEnd(int _targetLane, float _inputTime, float _judgeTime, int _stateIndex)
        {
            if (_inputTime - _judgeTime >= -0.15f && _inputTime - _judgeTime <= 0.15f)
            {
                _noteCounters[_stateIndex, _targetLane]++;
                _playerManager._players[playerID].noteSimpleCount++;
                judgeTextAnimators[(int) Judge.PERFECT].Play("Judge", 0, 0.0f);
                Debug.Log("Perfect");
            }
            else if (-0.2f <= _inputTime - _judgeTime && _inputTime - _judgeTime <= 0.2f)
            {
                _noteCounters[_stateIndex, _targetLane]++;
                _playerManager._players[playerID].noteSimpleCount++;
                judgeTextAnimators[(int) Judge.GOOD].Play("Judge", 0, 0.0f);
                Debug.Log("Good");
            }
            else if (_inputTime - _judgeTime >= -0.6f && _inputTime - _judgeTime <= 0.6f)
            {
                _noteCounters[_stateIndex, _targetLane]++;
                _playerManager._players[playerID].noteSimpleCount++;
                judgeTextAnimators[(int) Judge.BAD].Play("Judge", 0, 0.0f);
                Debug.Log("Bad");
            }
            else
            {
                _noteCounters[_stateIndex, _targetLane]++;
                _playerManager._players[playerID].noteSimpleCount++;
                judgeTextAnimators[(int) Judge.MISS].Play("Judge", 0, 0.0f);
                Debug.LogError("Miss");
            }
            
            //判定したときは現在のステートのNoteObjectをRemoveする
            if (_stateIndex == 1)
            {
                _fumenDataManager.mainNotes[playerID].RemoveAt(0);
            }
            else if (_stateIndex == 2)
            {
                _fumenDataManager.moreDifficultNotes[playerID].RemoveAt(0);
            }

            isLongNoteStart[_targetLane] = false;
        }

        //フリックノーツの判定
        //フリックジェスチャによるイベント
        private void OnGetPlayerGesture(int _playerId, PlayerGesture _gesture)
        {
            if (!isStartMusic)
            {
                return;
            }
            
            if (_gesture != PlayerGesture.NONE)
            {
//                Debug.Log("GetsGesture!" + ":" + _gesture);
                for (int i = 0; i < 5; i++)
                {
                    //裏で流しておく現在とは違う難易度の譜面については判定処理をスルーする。
                    switch (_playerManager._players[playerID].FumenState)
                    {
                        case PlayerFumenState.DEFAULT:
                            checkJudgeFlickNote(_gesture, _fumenDataManager.timings[_playerId, i],
                                PlayerFumenState.DEFAULT);
                            break;
                        case PlayerFumenState.MORE_DIFFICULT:
                            checkJudgeFlickNote(_gesture, _fumenDataManager.moreDifficultTimings[_playerId, i],
                                PlayerFumenState.MORE_DIFFICULT);
/*
                            checkSingleJudge(i, _fumenDataManager.timings[playerID, i], PlayerFumenState.MORE_DIFFICULT);
*/
                            break;
                        default:
                            break;
                    }                 
                    
                }
/*
                checkJudgeFlickNote(_gesture);
*/
            }
        }
        //メモ
        //基本的な流れは
        //1checkOOjudge系の関数にtimings,moreDifficultTimingsなどの各難易度の譜面タイミング情報を引数で渡す
        //2そこから現在のレーン情報をみる
        private void checkJudgeFlickNote(PlayerGesture _playerGesture,
            List<FumenDataManager.NoteTimingInfomation> targetTimings, PlayerFumenState fumenState)
        {
            var stateIndex = (int) fumenState;
         
            //もうリストがなくなりきってたらはじく
            if ((stateIndex == 1 && _fumenDataManager.mainNotes[playerID].Count == 0) || 
                (stateIndex == 2 && _fumenDataManager.moreDifficultNotes[playerID].Count == 0)) 
            {
                //Debug.Log("isNone");
                return;
            }
            
            for (int i = 0; i < 5; i++)
            {
                //押下したキーに対応するレーンに流れるすべてのノーツ情報。
                var targetLaneNoteInfos = targetTimings;
                
                //Debug.Log(targetLaneNoteInfos.Count+"   ___   "+_noteCounters[stateIndex, i]);

                
                //レーンのカウント数が最大数と同じであればはじく
                if (targetLaneNoteInfos.Count <= _noteCounters[stateIndex, i])
                {
                    continue;
                }

                //現在の次に来るはずのノーツ情報
                var nextNoteTimingInfo = targetLaneNoteInfos[_noteCounters[stateIndex, i]];
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
                        _noteCounters[stateIndex, i]++;
                        _playerManager._players[playerID].noteSimpleCount++;
                        judgeTextAnimators[(int) Judge.PERFECT].Play("Judge", 0, 0.0f);
                        Debug.Log("Perfect");
                    }
                    else if (_playerGesture != PlayerGesture.LEFT
                             && inputTime - judgeTime >= -0.6f && inputTime - judgeTime <= 0.6f)
                    {
                        _noteCounters[stateIndex, i]++;
                        _playerManager._players[playerID].noteSimpleCount++;
                        judgeTextAnimators[(int) Judge.BAD].Play("Judge", 0, 0.0f);
                        Debug.Log("Bad");
                    }
                    else
                    {

                    }
                    
                    //判定したときは現在のステートのNoteObjectをRemoveする
                    if ((int)fumenState == 1)
                    {
                        _fumenDataManager.mainNotes[playerID].RemoveAt(0);
                    }
                    else if ((int)fumenState == 2)
                    {
                        _fumenDataManager.moreDifficultNotes[playerID].RemoveAt(0);
                    }
                }
                //右フリックだった時
                else if (nextNoteTimingInfo.noteType == 4)
                {
                    //ジェスチャがLeftでなかったらBad
                    if (_playerGesture == PlayerGesture.RIGHT &&
                        inputTime - judgeTime >= -0.6f && inputTime - judgeTime <= 0.6f)
                    {
                        _noteCounters[stateIndex, i]++;
                        _playerManager._players[playerID].noteSimpleCount++;
                        judgeTextAnimators[(int) Judge.PERFECT].Play("Judge", 0, 0.0f);
                        Debug.Log("Perfect");
                    }
                    else if (_playerGesture != PlayerGesture.RIGHT
                             && inputTime - judgeTime >= -0.6f && inputTime - judgeTime <= 0.6f)
                    {
                        _noteCounters[stateIndex, i]++;
                        _playerManager._players[playerID].noteSimpleCount++;
                        judgeTextAnimators[(int) Judge.BAD].Play("Judge", 0, 0.0f);
                        Debug.Log("Bad");
                    }
                    else
                    {

                    }
                    //判定したときは現在のステートのNoteObjectをRemoveする
                    if ((int)fumenState == 1)
                    {
                        _fumenDataManager.mainNotes[playerID].RemoveAt(0);
                    }
                    else if ((int)fumenState == 2)
                    {
                        _fumenDataManager.moreDifficultNotes[playerID].RemoveAt(0);
                    }
                }

                //エラー回避
                Debug.LogWarning(targetLaneNoteInfos[_noteCounters[stateIndex,2]].noteType);

                if (_noteCounters[stateIndex, 2] >= targetTimings.Count)
                {
                    return;
                }

                //次のノーツが音札ノーツであればターンチェックのコルーチンを走らせ始める
                if (targetLaneNoteInfos[_noteCounters[stateIndex, 2]].noteType == 5)
                {
                    _playerManager.runCoroutine();
                }

            }

        }


        //音札ノーツの判定
        private void OnGetPlayerBuddhaGesture(int _playerID)
        {
/*
            Debug.Log("getBuddha");
*/

            if (playerID != _playerID)
            {
                return;
            }
            for (int i = 0; i < 5 ; i++)
            {
                laneLight[i].SetActive(true);
/*
                Debug.Log("player:"+_playerID +"active "+i);
*/
            }
            
            if (!isStartMusic)
            {
                Debug.Log("is NOT Start Music");
                return;
            }
            
            //裏で流しておく現在とは違う難易度の譜面については判定処理をスルーする。
            switch (_playerManager._players[playerID].FumenState)
            {
                case PlayerFumenState.DEFAULT: 
                    checkOtoFudaJudge(_fumenDataManager.timings[playerID, 2],PlayerFumenState.DEFAULT);
                    break;
                case PlayerFumenState.MORE_DIFFICULT:
                    checkOtoFudaJudge(_fumenDataManager.moreDifficultTimings[playerID, 2],PlayerFumenState.MORE_DIFFICULT);
                    break;
                default:
                    break;
            }   
        }

        //手をたたいた時
        private void checkOtoFudaJudge(List<FumenDataManager.NoteTimingInfomation> targetTimings,
            PlayerFumenState fumenState)
        {
            if (!isStartMusic)
            {
                return;
            }
            
            var stateIndex = (int) fumenState;
            
            //stateIndexがデフォルトを示すものでない場合、デフォルトに戻す
            //ターンの開始兼終わりであるため
            if (stateIndex != 1)
            {
                stateIndex = 1;
                _playerManager._players[playerID].FumenState = PlayerFumenState.DEFAULT;
            }
            
            //押下したキーに対応するレーンに流れるすべてのノーツ情報。
            var targetLaneNoteInfos = targetTimings;

            //レーンのカウント数が最大数と同じであればはじく
            if (targetLaneNoteInfos.Count <= _noteCounters[stateIndex,2])
            {
                return;
            }

            //現在の次に来るはずのノーツ情報
            var nextNoteTimingInfo = targetLaneNoteInfos[_noteCounters[stateIndex,2]];

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
                _noteCounters[stateIndex,2]++;
                _playerManager._players[playerID].noteSimpleCount++;
                judgeTextAnimators[(int) Judge.PERFECT].Play("Judge", 0, 0.0f);
                Debug.Log("Perfect");

                //音札を利用したというアクションを発火
                OnUseOtoFudaCard?.Invoke(playerID);
            }
            else
            {
            }
            
            //判定したときは現在のステートのNoteObjectをRemoveする
            if ((int)fumenState == 1)
            {
                _fumenDataManager.mainNotes[playerID].RemoveAt(0);
            }
            else if ((int)fumenState == 2)
            {
                _fumenDataManager.moreDifficultNotes[playerID].RemoveAt(0);
            }

            //エラー回避
            if (_noteCounters[stateIndex, 2] >= _fumenDataManager.timings[playerID, 2].Count)
            {
                return;
            }
            Debug.LogWarning(targetLaneNoteInfos[_noteCounters[stateIndex,2]].noteType);

            //次のノーツが音札ノーツであればターンチェックのコルーチンを走らせ始める
            if (targetLaneNoteInfos[_noteCounters[stateIndex,2]].noteType == 5)
            {
                _playerManager.runCoroutine();
            }
        }

        //手を離したとき
        //判定ラインの光を消す処理
        private void OnReleasePlayerBuddhaPalm(int _playerID)
        {
/*
            Debug.Log("releaseBuddha");
*/

            if (playerID != _playerID)
            {
                return;
            }
            
            for (int i = 0; i < 5; i++)
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
