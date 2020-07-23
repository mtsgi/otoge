using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using OtoFuda.Fumen;
using MiniJSON;
using OtoFuda.player;
using UnityEngine;
using Object = UnityEngine.Object;

//難易度をEnumで定義
public enum GameDifficulty
{
    Easy = 0,
    Normal = 1,
    Hard = 2
}

public class JsonReadManager
{
    private int playerID;
    private string fileName;

    public bool isDebug = false;


    public GameDifficulty GameDifficulty = GameDifficulty.Normal;

    private FumenDataManager _fumenDataManager;
    private MusicData _musicData;

    public JsonReadManager(MusicData musicData)
    {
        _musicData = musicData;
    }

    public void Init(FumenDataManager fumenDataManager, int playerId)
    {
        /*		if (!isDebug)
		{*/
        if (_musicData == null)
        {
            Debug.LogError("MusicDataが空です!!");
        }
        else
        {
            playerID = playerId;
            fileName = _musicData.jsonFilePath;
            GameDifficulty = _musicData.levels[playerID];

            Debug.Log($"MusicData Info :FilePath{_musicData.jsonFilePath}/BPM{_musicData.bpm}" +
                      $"/beat{_musicData.beat}" +
                      $"/Level{_musicData.levels[playerID]}/musicID{_musicData.musicId}" +
                      $"/Offset{_musicData.offset}");
        }

/*        if (!(SceneLoadManager.Instance.previewSceneTransitionData is MusicSelectManager.MusicData musicData))
        {
            musicData = new MusicSelectManager.MusicData();
            Debug.Log($"musicDa{}");
        }*/


        Debug.Log("player1 LEVEL is " + GameDifficulty);
//		}

//        _fumenDataManager = FumenDataManager.Instance;
        _fumenDataManager = fumenDataManager;
        SerializeFumenData();
    }

    private void SerializeFumenData()
    {
        var textAsset = Resources.Load("FumenJsons/" + fileName) as TextAsset;
        if (textAsset != null)
        {
            var jsonText = textAsset.text;
            var fumen = JsonUtility.FromJson<FumenInfo>(jsonText);
            SetNoteData(fumen);
            Debug.Log($"Fumen {fileName} is loaded!");
        }
        else
        {
            Debug.LogError($"Fumen {fileName} is not found!");
        }
    }

    private void SetNoteData(FumenInfo fumenInfo)
    {
        //難易度に応じて全てのノーツを生成し、FumenDataManagerのMainNotesの中にしまう。
        switch (GameDifficulty)
        {
            case GameDifficulty.Easy:
                //イージー難易度のノーツを生成
                for (int i = 0; i < fumenInfo.easy.Count; i++)
                {
                    NoteGenerate(_fumenDataManager.mainNotes[playerID], _fumenDataManager.timings,
                        fumenInfo.easy[i], _musicData.bpm, _musicData.beat, 0);
/*
					_fumenDataManager.mainNotes.Add(_note);
*/
                }

                //ノーマル難易度のノーツを生成
                for (int i = 0; i < fumenInfo.normal.Count; i++)
                {
                    NoteGenerate(_fumenDataManager.moreDifficultNotes[playerID],
                        _fumenDataManager.moreDifficultTimings,
                        fumenInfo.normal[i], _musicData.bpm, _musicData.beat, 1);
/*
					_fumenDataManager.mainNotes.Add(_note);
*/
                }

                break;

            case GameDifficulty.Normal:
                //ノーマル難易度のノーツを生成
                for (int i = 0; i < fumenInfo.normal.Count; i++)
                {
                    NoteGenerate(_fumenDataManager.mainNotes[playerID], _fumenDataManager.timings,
                        fumenInfo.normal[i], _musicData.bpm, _musicData.beat, 0);
/*
					_fumenDataManager.mainNotes.Add(_note);
*/
                }

//				Debug.Log(fumenInfo.normal.Count);
                //ハード難易度のノーツを生成
                for (int i = 0; i < fumenInfo.hard.Count; i++)
                {
                    NoteGenerate(_fumenDataManager.moreDifficultNotes[playerID],
                        _fumenDataManager.moreDifficultTimings,
                        fumenInfo.hard[i], _musicData.bpm, _musicData.beat, 1);
/*
					_fumenDataManager.mainNotes.Add(_note);
*/
                }
/*
				Debug.Log(fumenInfo.hard.Count);
*/

                break;

            case GameDifficulty.Hard:

                //ハード難易度のノーツを生成
                for (int i = 0; i < fumenInfo.hard.Count; i++)
                {
                    NoteGenerate(_fumenDataManager.mainNotes[playerID], _fumenDataManager.timings,
                        fumenInfo.hard[i], _musicData.bpm, _musicData.beat, 0);
/*
					_fumenDataManager.mainNotes.Add(_note);
*/
                }

                //エクストラ難易度のノーツを生成
                for (int i = 0; i < fumenInfo.extra.Count; i++)
                {
                    NoteGenerate(_fumenDataManager.moreDifficultNotes[playerID],
                        _fumenDataManager.moreDifficultTimings,
                        fumenInfo.extra[i], _musicData.bpm, _musicData.beat, 1);
/*
					_fumenDataManager.mainNotes.Add(_note);
*/
                }


                break;
        }

        //ノーツのリストをソートする
        _fumenDataManager.mainNotes[playerID].Sort((x, y) => x.reachFrame.CompareTo(y.reachFrame));
        _fumenDataManager.moreDifficultNotes[playerID].Sort((x, y) => x.reachFrame.CompareTo(y.reachFrame));


        for (int i = 0; i < 5; i++)
        {
            _fumenDataManager.timings[playerID, i].Sort((x, y) => x.reachTime.CompareTo(y.reachTime));
            _fumenDataManager.moreDifficultTimings[playerID, i].Sort((x, y) => x.reachTime.CompareTo(y.reachTime));
        }
    }


    //ノーツの生成処理
    private void NoteGenerate(List<NoteObject> targetNotesList,
        List<FumenDataManager.NoteTimingInformation>[,] targetTimingList,
        NotesInfo notesInfo, float _bpm, float _beat, float _ZoffSet)
    {
        //ノーツのGameObject
        GameObject noteGameObject = null;
        var spawnPos = Vector3.zero;

        //ロングノーツ時、endノーツのFumenDataManagerのMainNotesの中でのIndexを格納する
        int endNoteIndex = -1;

        int lane = notesInfo.lane;

        //ノーツのタイプによって生成するオブジェクトを変える。
        switch (notesInfo.type)
        {
            case 0:
                break;
            case 1:
                noteGameObject = (GameObject) Resources.Load("NoteObjects/Prefabs/NormalNote");
                break;
            case 2:
                noteGameObject = (GameObject) Resources.Load("NoteObjects/Prefabs/NormalNote");
                break;
            case 3:
                noteGameObject = (GameObject) Resources.Load("NoteObjects/Prefabs/Flick_L");
                break;
            case 4:
                noteGameObject = (GameObject) Resources.Load("NoteObjects/Prefabs/Flick_R");
                break;
            case 5:
                noteGameObject = (GameObject) Resources.Load("NoteObjects/Prefabs/OtofudaNote");
                lane = 3;
                break;

            case 99:
                noteGameObject = (GameObject) Resources.Load("NoteObjects/Prefabs/OtherNote");
                break;
            //エラーが怖いのでなんもないときはとりあえずOtherを立ててエラーログを吐く
            default:
                noteGameObject = (GameObject) Resources.Load("NoteObjects/Prefabs/OtherNote");
                Debug.LogError("ノーツ生成時にエラーが発生しました。");
                break;
        }


        //一小節あたりの長さ(秒)
        //60 秒/_bpm (拍)で 1拍 あたり何秒なのかを算出。
        //これにbeat(拍子数)をかけることで一小節あたりの時間を計算する。
        var measureLength = (60 / _bpm) * _beat;

        /*Debug.Log($"{60}/{_bpm}*{_beat}");*/


        //一小節あたりの時間がわかれば何秒で到達するノーツなのかを算出できる。
        var reachFrame = measureLength * ((float) notesInfo.measure - 1) +
                         measureLength * ((float) notesInfo.position / (float) notesInfo.split) +
                         (_musicData.offset * 0.001f);

        //laneの長さ
        var _laneLength = _fumenDataManager.laneLength;

        spawnPos = new Vector3(lane + (playerID * 20),
            reachFrame * _laneLength * _fumenDataManager._highSpeed[playerID],
            0 + _ZoffSet);


        //生成
        var spawnedObject = Object.Instantiate(noteGameObject, spawnPos, Quaternion.identity);
//        Debug.Log(spawnedObject.transform.position);
        spawnedObject.transform.parent = GameObject.Find("Notes").transform;

        var _noteObject = spawnedObject.GetComponent<NoteObject>();

        if (_noteObject == null)
        {
            Debug.LogError("ノーツのオブジェクトにnoteObjectのスクリプトがアタッチされていませんでした");
            return;
        }


        //ノーツ本体のスクリプトに値を格納
        _noteObject.SetNoteObject(notesInfo.type, lane, endNoteIndex, notesInfo.option, reachFrame, playerID,
            _fumenDataManager._highSpeed[playerID], _laneLength);


/*
		Debug.Log(FumenDataManager.Instance.mainNotes[0].Count);
*/
        targetNotesList.Add(_noteObject);

        //タイミング情報だけを格納して扱いやすくする
        if (lane > 0)
        {
            targetTimingList[playerID, lane - 1]
                .Add(new FumenDataManager.NoteTimingInformation(notesInfo.type, reachFrame));
        }

        if (notesInfo.type == 5)
        {
//			Debug.Log("OTOFUDA NOTE " + spawnedObject.transform.position);
        }


        //endノーツが含まれていた場合、さらに生成してmainNotesの中につっこむ
        for (int i = 0; i < notesInfo.end.Count; i++)
        {
            var spawnX = spawnPos.x;

            var spawnZ = 0f;

            //終点ノーツの生成
            NoteGenerate(targetNotesList, targetTimingList, notesInfo.end[i], _bpm, _beat, _ZoffSet);
            //longNoteの終点ノーツを追加した直後なのでもっとも後ろのIndexが終点ノーツを格納したindex
            endNoteIndex = targetNotesList.Count - 1;

            //終点情報を更新
            targetNotesList[endNoteIndex - 1]._endNotesNum = endNoteIndex;

            //終点座標からロングノーツのラインの生成座標を計算する。
            var spawnY =
                (targetNotesList[endNoteIndex].reachFrame * _laneLength *
                    _fumenDataManager._highSpeed[playerID] + spawnPos.y) / 2;

            var extend = targetNotesList[endNoteIndex].reachFrame * _laneLength *
                _fumenDataManager._highSpeed[playerID] - spawnPos.y;

            GameObject longLine = (GameObject) Resources.Load("NoteObjects/Prefabs/testLongNote");


            var longLinePos = new Vector3(spawnX, spawnY, _ZoffSet);
            var scale = new Vector3(0.1f, 0.018f * extend * 9, 1);
/*
			Debug.Log(extend);
*/
            longLine.GetComponent<SpriteRenderer>().drawMode = SpriteDrawMode.Sliced;
            longLine.GetComponent<SpriteRenderer>().size = new Vector2(7f, 10.0f * extend);
            /*longLine.transform.localScale = scale;*/

            var spawnedLongObject = Object.Instantiate(longLine, longLinePos, Quaternion.identity);
            spawnedLongObject.transform.parent = GameObject.Find("Notes/").transform;
            spawnedLongObject.transform.parent = targetNotesList[endNoteIndex].gameObject.transform;
        }
    }


    [ContextMenu("test")]
    private void test()
    {
        var textAsset = Resources.Load("FumenJsons/" + fileName) as TextAsset;
        var jsonText = textAsset.text;

        var fumenRaku = JsonUtility.FromJson<FumenInfo>(jsonText);
        Debug.Log(fumenRaku.easy[0].end[0].type); //114514
    }
}