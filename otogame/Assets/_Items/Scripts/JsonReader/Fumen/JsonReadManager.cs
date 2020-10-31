using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
    Hard = 2,
    End
}

public class JsonReadManager
{
    private int playerID;
    private string fileName;

    public bool isDebug = false;


    public GameDifficulty GameDifficulty = GameDifficulty.Normal;

    private FumenDataManager _fumenDataManager;
    private MusicData _musicData;

    private Transform _notesRootTransform;

    private NotesInfo eofNote;

    public JsonReadManager(MusicData musicData, Transform rootTransform)
    {
        _musicData = musicData;
        _notesRootTransform = rootTransform;
    }

    public void Init(FumenDataManager fumenDataManager, int playerId, FumenDataManager.MusicDataMode mode)
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

            Debug.Log($"MusicData Info :FilePath[{_musicData.jsonFilePath}]" +
                      $"/BPM[{_musicData.bpm}]" +
                      $"/beat[{_musicData.beat}]" +
                      $"/Level[{_musicData.levels[playerID]}]" +
                      $"/musicID[{_musicData.musicId}]" +
                      $"/Offset[{_musicData.offset}]");
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

        var jsonPath = "";
        if (mode == FumenDataManager.MusicDataMode.Game || mode == FumenDataManager.MusicDataMode.Debug)
        {
            Debug.Log(fileName);
            /*jsonPath = Application.streamingAssetsPath + "/FumenJsons/" + fileName + ".json";
            jsonPath = Application.streamingAssetsPath + "/FumenJsons/" + fileName + ".json";*/
            /*jsonPath = Resources.Load<TextAsset>("FumenJsons/" + fileName).ToString();*/
            jsonPath = "FumenJsons/" + fileName;
        }
        else if (mode == FumenDataManager.MusicDataMode.AutoPlay)
        {
            jsonPath = fileName;
        }

        SerializeFumenData(jsonPath);
    }

    private void SerializeFumenData(string jsonPath)
    {
        if (string.IsNullOrEmpty(jsonPath))
        {
            return;
        }

        //いっかいオミットしておく
        /*if (!File.Exists(jsonPath))
        {
            Debug.LogError("Json Not Found");
            return;
        }

        Debug.Log(jsonPath);
        using (var fs = new FileStream(jsonPath, FileMode.Open))
        {
            using (var sr = new StreamReader(fs))
            {
                var jsonText = sr.ReadToEnd();
                var fumen = Utf8Json.JsonSerializer.Deserialize<FumenInfo>(jsonText);
                SetNoteData(fumen);
                Debug.Log($"Fumen {fileName} is loaded!");
            }
        }*/
        var jsonText = "";
        using (var fs = new FileStream(jsonPath, FileMode.Open))
        {
            using (var sr = new StreamReader(fs))
            {
                jsonText = sr.ReadToEnd();
            }
        }

        Debug.Log(jsonPath);
//        var jsonText = Resources.Load<TextAsset>(jsonPath).ToString();
        if (string.IsNullOrEmpty(jsonText))
        {
            Debug.LogError("Json Not Found");
            return;
        }

        var fumen = Utf8Json.JsonSerializer.Deserialize<FumenInfo>(jsonText);
        SetNoteData(fumen);
    }

    private void SetNoteData(FumenInfo fumenInfo)
    {
        if (fumenInfo.info != null)
        {
            _musicData.bpm = fumenInfo.info.bpm;
            _musicData.beat = fumenInfo.info.beat;
            _musicData.offset = fumenInfo.info.offset;
            /*
            Debug.Log("Music Data is Updated");
        */
        }

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

                Debug.Log(fumenInfo.hard.Count);
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

        BeatLineGenerate();
    }

    private void BeatLineGenerate()
    {
        for (int i = 0; i < eofNote.measure; i++)
        {
            var beatLineInfo = new NotesInfo();
            beatLineInfo.type = -1;
            beatLineInfo.measure = i;
            beatLineInfo.lane = 3;
            beatLineInfo.position = 0;
            beatLineInfo.split = 4;
            beatLineInfo.option = new float[0];
            beatLineInfo.end = new List<NotesInfo>();

            NoteGenerate(_fumenDataManager.mainNotes[playerID], _fumenDataManager.timings,
                beatLineInfo, _musicData.bpm, _musicData.beat, 0);
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
                Debug.Log("Break");
                break;
            case -1:
                noteGameObject = (GameObject) Resources.Load("NoteObjects/Prefabs/BeatLine");
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
            case 95:
                Debug.LogError($" {notesInfo.type} 生成");
                return;
                noteGameObject = (GameObject) Resources.Load("NoteObjects/Prefabs/BeatLine");
                lane = 3;
                break;
            case 99:
                noteGameObject = (GameObject) Resources.Load("NoteObjects/Prefabs/OtherNote");
                noteGameObject.gameObject.name = "EndNote";
                break;
            //エラーが怖いのでなんもないときはとりあえずOtherを立ててエラーログを吐く
            default:
                Debug.LogError($"ノーツ生成時にエラーが発生しました。 {notesInfo.type}");
                return;
                break;
        }


        //一小節あたりの(秒)
        //60 秒/_bpm (拍)で 1拍 あたり何秒なのかを算出。
        //これにbeat(拍子数)をかけることで一小節あたりの時間を計算する。
        var measureLength = (60.0f / _bpm) * _beat;

        /*Debug.Log($"{60}/{_bpm}*{_beat}");*/


        //一小節あたりの時間がわかれば何秒で到達するノーツなのかを算出できる。
        var reachTime = measureLength * ((float) notesInfo.measure - 1) +
                        measureLength * ((float) notesInfo.position / (float) notesInfo.split) +
                        (_musicData.offset * 0.001f);

        //laneの長さ
        var _laneLength = _fumenDataManager.laneLength;

        spawnPos = new Vector3(lane + (playerID * 20),
            reachTime * _laneLength * _fumenDataManager._highSpeed[playerID],
            0 + _ZoffSet);


        //生成
        var spawnedObject = Object.Instantiate(noteGameObject, spawnPos, Quaternion.identity);
//        Debug.Log(spawnedObject.transform.position);
        if (_notesRootTransform != null)
        {
            spawnedObject.transform.parent = _notesRootTransform;
        }
        else
        {
            spawnedObject.transform.parent = GameObject.Find("Notes").transform;
        }

        var _noteObject = spawnedObject.GetComponent<NoteObject>();

        if (_noteObject == null)
        {
            Debug.LogError("ノーツのオブジェクトにnoteObjectのスクリプトがアタッチされていませんでした");
            return;
        }


        //ノーツ本体のスクリプトに値を格納
        _noteObject.SetNoteObject(notesInfo.type, lane, endNoteIndex, notesInfo.option, reachTime, playerID,
            _fumenDataManager._highSpeed[playerID], _laneLength);

        targetNotesList.Add(_noteObject);


        if (notesInfo.type == -1)
        {
            var renderer = spawnedObject.GetComponent<SpriteRenderer>();
            var sliceSize = renderer.size;
            renderer.drawMode = SpriteDrawMode.Sliced;
            renderer.size = new Vector2(sliceSize.x * 5, sliceSize.y);
        }


        //タイミング情報だけを格納して扱いやすくする
        if (lane > 0)
        {
            targetTimingList[playerID, lane - 1]
                .Add(new FumenDataManager.NoteTimingInformation(notesInfo.type, reachTime));
        }

        if (notesInfo.type == 5)
        {
//			Debug.Log("OTOFUDA NOTE " + spawnedObject.transform.position);
        }


        //フリックのサイズ変更・オフセット変更
        if (notesInfo.type == 3 || notesInfo.type == 4)
        {
            var ratio = 3.0f;

            if (notesInfo.option.Length != 0)
            {
                //-1の時は3倍(デフォ)
                if (notesInfo.option[0] == -1)
                {
                    ratio = 3;
                }
                else
                {
                    ratio = notesInfo.option[0];
                    if (notesInfo.option.Length >= 3)
                    {
                        //オフセットを計算して座標を更新
                        //1レーンの長さ=1.0fとしてある
                        var flickOffset = 1.0f * ((float) notesInfo.option[1] / (float) notesInfo.option[2]);
                        spawnPos.x += flickOffset;
                        spawnedObject.transform.localPosition = spawnPos;
                    }
                }
            }

            var renderer = spawnedObject.GetComponent<SpriteRenderer>();
            var sliceSize = renderer.size;
            renderer.drawMode = SpriteDrawMode.Sliced;
            renderer.size = new Vector2(sliceSize.x * ratio, sliceSize.y);
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

        //EOFノーツを外部に格納する。
        if (notesInfo.type == 99)
        {
            eofNote = notesInfo;
        }
    }


    /*[ContextMenu("test")]
    private void test()
    {
        var textAsset = Resources.Load("FumenJsons/" + fileName) as TextAsset;
        var jsonText = textAsset.text;

        var fumenRaku = JsonUtility.FromJson<FumenInfo>(jsonText);
        Debug.Log(fumenRaku.easy[0].end[0].type); //114514
    }*/
}