using System;
using System.Collections.Generic;
using System.Linq;
using OtoFuda.Fumen;
using UnityEngine;
using Object = UnityEngine.Object;

public class FumenNoteGenerator
{
    private MusicData _musicData;
    private Transform _notesRootTransform;
    private float _laneLength;

    public FumenNoteGenerator(Transform notesRootTransform, MusicData musicData, float laneLength)
    {
        _notesRootTransform = notesRootTransform;
        _musicData = musicData;
        _laneLength = laneLength;
    }

    //FumenInfoからnoteObjectのリストを返却する。
    public List<NoteObject> GenerateNotesData(FumenInfo fumenInfo, GameDifficulty difficulty,
        int playerID, float hiSpeed, float zOffset)
    {
        var result = new List<NoteObject>();
        if (fumenInfo.info != null)
        {
            _musicData.bpm = fumenInfo.info.bpm;
            _musicData.beat = fumenInfo.info.beat;
            _musicData.offset = fumenInfo.info.offset;
            /*
            Debug.Log("Music Data is Updated");
        */
        }

        var tmpBpm = _musicData.bpm;
        var tmpBeat = _musicData.beat;

        //todo あくまで行うのは生成まで。気を利かせて譜面変更時のノーツまで生成してあげる必要はない
        //難易度に応じて全てのノーツを生成し、FumenDataManagerのMainNotesの中にしまう。
        switch (difficulty)
        {
            case GameDifficulty.Easy:
                //Easy難易度のノーツを生成
                for (int i = 0; i < fumenInfo.easy.Count; i++)
                {
                    if (fumenInfo.easy[i].type != 95)
                    {
                        NoteGenerate(result, playerID, hiSpeed, fumenInfo.easy[i],
                            ref tmpBpm, ref tmpBeat, zOffset);
                    }
                }

                break;

            case GameDifficulty.Normal:
                //Easy難易度のノーツを生成
                for (int i = 0; i < fumenInfo.normal.Count; i++)
                {
                    if (fumenInfo.normal[i].type != 95)
                    {
                        NoteGenerate(result, playerID, hiSpeed, fumenInfo.normal[i],
                            ref tmpBpm, ref tmpBeat, zOffset);
                    }
                }

                break;

            case GameDifficulty.Hard:
                Debug.Log(fumenInfo.hard.Count);
                //ハード難易度のノーツを生成
                for (int i = 0; i < fumenInfo.hard.Count; i++)
                {
                    if (fumenInfo.hard[i].type != 95)
                    {
                        NoteGenerate(result, playerID, hiSpeed, fumenInfo.hard[i],
                            ref tmpBpm, ref tmpBeat, zOffset);
                    }
                }

                break;
        }

        return result;
    }


    public List<NoteObject> BeatLineGenerate(FumenInfo fumenInfo, GameDifficulty difficulty,
        int playerID, float hiSpeed, float zOffSet, NoteObject eofNoteObject)
    {
        var notesInfo = new List<NotesInfo>();
        switch (difficulty)
        {
            case GameDifficulty.Easy:
                notesInfo = fumenInfo.easy;
                break;
            case GameDifficulty.Normal:
                notesInfo = fumenInfo.normal;
                break;
            case GameDifficulty.Hard:
                notesInfo = fumenInfo.hard;
                break;
        }

        var tmpBpm = _musicData.bpm;
        var bpmChangeCount = 0;
        var type98Notes = notesInfo.Where(x => x.type == 98).ToArray();

        var tmpBeat = _musicData.beat;
        var beatChangeCount = 0;
        var type97Notes = notesInfo.Where(x => x.type == 97).ToArray();


        //EOFノーツを探す
        if (eofNoteObject != null)
        {
            var result = new List<NoteObject>();

            for (int i = 0; i < eofNoteObject._measure; i++)
            {
                //同位置のtype95のノーツを探して通常小節線と同じ位置のものがあれば生成しない
                var i1 = i;
                var type95Note = notesInfo.FirstOrDefault(x =>
                    x.type == 95 && x.position == 0 && x.measure == i1);

                //発見されたtype95のOptionが-1であれば生成しない
                if (type95Note == null || type95Note.option.Length == 0 || (int) type95Note.option[0] != -1)
                {
                    if (beatChangeCount != type97Notes.Length)
                    {
                        //現在の小節数よりも次type97Noteの小節数が大きければ
                        if (i <= type97Notes[beatChangeCount].measure)
                        {
                            tmpBeat = (int) type97Notes[beatChangeCount].option[0];
                            beatChangeCount++;
                        }
                    }


                    if (bpmChangeCount != type98Notes.Length)
                    {
                        //現在の小節数よりも次type97Noteの小節数が大きければ
                        if (i <= type98Notes[bpmChangeCount].measure)
                        {
                            tmpBpm = (int) type98Notes[bpmChangeCount].option[0];
                            bpmChangeCount++;
                        }
                    }


                    var beatLineInfo = new NotesInfo();
                    beatLineInfo.type = -1;
                    beatLineInfo.measure = i;
                    beatLineInfo.lane = 3;
                    beatLineInfo.position = 0;
                    beatLineInfo.split = 4;
                    beatLineInfo.option = new float[0];
                    beatLineInfo.end = new List<NotesInfo>();

                    NoteGenerate(result, playerID, hiSpeed, beatLineInfo,
                        ref tmpBpm, ref tmpBeat, zOffSet);
                }
            }

            //type95でかつoptionが-1でないもの、つまり特殊小節線ノーツを探す
            var type95Notes = notesInfo.Where(x =>
                x.type == 95 && x.option.Length != 0 && (int) x.option[0] != -1).ToArray();

            for (int i = 0; i < type95Notes.Length; i++)
            {
                NoteGenerate(result, playerID, hiSpeed, type95Notes[i],
                    ref tmpBpm, ref tmpBeat, zOffSet);
            }

            return result;
        }


        return null;
    }


    //ノーツの生成処理
    //todo PlayerIdとかHiSpeedってどっかにまとめたオブジェクトがなかったけ？
    /// <summary>
    /// 
    /// </summary>
    /// <param name="noteObjectsList"></param>
    /// <param name="playerId">どのIdのPlayerの画面上に生成するか？</param>
    /// <param name="hiSpeed">そのPlayerのハイスピ</param>
    /// <param name="notesInfo"></param>
    /// <param name="bpm"></param>
    /// <param name="beat"></param>
    /// <param name="zOffSet"></param>
    private NoteObject NoteGenerate(
        List<NoteObject> noteObjectsList, int playerId, float hiSpeed, NotesInfo notesInfo,
        ref float bpm, ref float beat, float zOffSet)
    {
        //ノーツのGameObject
        GameObject noteGameObject = null;

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
                noteGameObject = (GameObject) Resources.Load("NoteObjects/Prefabs/BeatLine");
                break;
            case 97:
                beat = (int) notesInfo.option[0];
                return null;
            case 98:
                bpm = (int) notesInfo.option[0];
                return null;
            case 99:
                noteGameObject = (GameObject) Resources.Load("NoteObjects/Prefabs/OtherNote");
                noteGameObject.gameObject.name = "EndNote";
                break;
            //エラーが怖いのでなんもないときはとりあえずOtherを立ててエラーログを吐く
            default:
                Debug.LogError($"ノーツ生成時にエラーが発生しました。 {notesInfo.type}");
                return null;
                break;
        }


        //一小節あたりの(秒)
        //60 秒/_bpm (拍)で 1拍 あたり何秒なのかを算出。
        //これにbeat(拍子数)をかけることで一小節あたりの時間を計算する。
        var measureLength = (60.0f / bpm) * beat;

        /*Debug.Log($"{60}/{_bpm}*{_beat}");*/


        //一小節あたりの時間がわかれば何秒で到達するノーツなのかを算出できる。
        var reachTime = measureLength * ((float) notesInfo.measure - 1) +
                        measureLength * ((float) notesInfo.position / (float) notesInfo.split) +
                        (_musicData.offset * 0.001f);

        var spawnPos = new Vector3(lane + (playerId * 20),
            reachTime * _laneLength * hiSpeed,
            0 + zOffSet);


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

        var noteObject = spawnedObject.GetComponent<NoteObject>();

        if (noteObject == null)
        {
            Debug.LogError("ノーツのオブジェクトにnoteObjectのスクリプトがアタッチされていませんでした");
            return null;
        }


        //ノーツ本体のスクリプトに値を格納
        noteObject.SetNoteObject(notesInfo.type, lane, endNoteIndex, notesInfo.option, reachTime, playerId,
            hiSpeed, _laneLength, notesInfo.measure);

        //targetNotesList.Add(noteObject);


        if (notesInfo.type == -1)
        {
            var renderer = spawnedObject.GetComponent<SpriteRenderer>();
            var sliceSize = renderer.size;
            renderer.drawMode = SpriteDrawMode.Sliced;
            renderer.size = new Vector2(sliceSize.x * 5, sliceSize.y);
        }


        //todo タイミング情報だけを格納しているのは多分譜面の判定処理を行いやすくするため
        //ちょっと保留
        //ここでやる必要性は？Notesのリストを返却しているのでこれを呼び出す側で操作してあげれば別にいいのでは？
        //ややこしくなる原因。
        //やっぱここで渡す方が楽。なぜなら、終点ノーツの中にさらにノーツが存在する、みたいな仕様を要求された場合
        //生成時の再帰処理に任せた方が楽
        //タイミング情報だけを格納して扱いやすくする
        //いる？？？？？mainをソートするだけでは正しく動かない？
        //結局参照だからここでほぼ同じ性質の別クラスとして立てる必要性は?
        //レーンごとの状態を手に入れる必要性はある。
        //これを呼び出す側が受け取ったmainNoteのリストをループで回してレーンごとにわける？
        //(mainNoteの中にChildも含まれるので問題はないはず…)
        /*if (lane > 0)
        {
            timingInfos[lane - 1].Add(new NoteTimingInformation(notesInfo.type, reachTime));
        }*/

        if (notesInfo.type == 5)
        {
//			Debug.Log("OTOFUDA NOTE " + spawnedObject.transform.position);
        }


        //フリックのサイズ変更・オフセット変更

        #region フリックの大きさ変更

        if (notesInfo.type == 3 || notesInfo.type == 4)
        {
            var ratio = 3.0f;

            if (notesInfo.option.Length != 0)
            {
                //-1の時は3倍(デフォ)
                if ((int) notesInfo.option[0] == -1)
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

        #endregion

        #region 特殊小節線の生成

        if (notesInfo.type == 95)
        {
            //-1の場合は何もしない
            if (notesInfo.option.Length != 0 && (int) notesInfo.option[0] != -1)
            {
                var ratio = notesInfo.option[0];
                if (notesInfo.option.Length >= 3)
                {
                    //オフセットを考慮して座標を更新
                    //1レーンの長さ=1.0fとしてある
                    var posX = notesInfo.lane + (ratio - 1 * 0.5f);
                    spawnPos.x += posX;
                    spawnedObject.transform.localPosition = spawnPos;

                    var renderer = spawnedObject.GetComponent<SpriteRenderer>();
                    var sliceSize = renderer.size;
                    renderer.drawMode = SpriteDrawMode.Sliced;
                    renderer.size = new Vector2(sliceSize.x * ratio, sliceSize.y);
                }
            }
        }

        #endregion

        #region 終点ノーツ生成

        //endノーツが含まれていた場合、さらに生成してmainNotesの中につっこむ
        if (notesInfo.end != null)
        {
            if (notesInfo.end.Count != 0)
            {
                noteObject.childNote = new List<NoteObject>();
                for (int i = 0; i < notesInfo.end.Count; i++)
                {
                    var spawnX = spawnPos.x;

                    //終点ノーツの生成
                    var childNote = NoteGenerate(noteObjectsList,
                        playerId, hiSpeed, notesInfo.end[i], ref bpm, ref beat, zOffSet);

                    noteObject.childNote.Add(childNote);

                    //終点座標からロングノーツのラインの生成座標を計算する。
                    //まず生成するY座標は終点ノーツの生成された座標と始点ノーツの生成された座標の中央
                    var spawnY = (childNote.reachFrame * _laneLength * hiSpeed + spawnPos.y) / 2;
                    //さらに2点の距離を求めて何倍引き伸ばすかを計算する
                    var extend = childNote.reachFrame * _laneLength * hiSpeed - spawnPos.y;
                    GameObject longLine = (GameObject) Resources.Load("NoteObjects/Prefabs/testLongNote");
                    var longLinePos = new Vector3(spawnX, spawnY, zOffSet);
                    var scale = new Vector3(0.1f, 0.018f * extend * 9, 1);

                    longLine.GetComponent<SpriteRenderer>().drawMode = SpriteDrawMode.Sliced;
                    longLine.GetComponent<SpriteRenderer>().size = new Vector2(7f, 10.0f * extend);

                    var spawnedLongObject = Object.Instantiate(longLine, longLinePos, Quaternion.identity);
                    spawnedLongObject.transform.parent = GameObject.Find("Notes/").transform;
                    //最終的にロングノーツの帯は終点ノーツの子オブジェクトとして存在させる。
                    //これは始点が流れていったあと非表示になるので、終点の子に登録しておかないとそのタイミングで同時に消えてしまうため。
                    spawnedLongObject.transform.parent = childNote.transform;
                }
            }
        }

        #endregion


        if (noteObject != null)
        {
            noteObjectsList.Add(noteObject);
        }

        return noteObject;
    }
}