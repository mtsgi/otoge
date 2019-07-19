using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using OtoFuda.Fumen;
using MiniJSON;
using OtoFuda.player;
using UnityEngine;

public class JsonReadManager : MonoBehaviour
{
	[Range(0,1)]
	[SerializeField] private int playerID;
	[SerializeField] private string fileName;

	//難易度をEnumで定義
	public enum DIFFICULTY
	{
		EASY,
		NORMAL,
		HARD
	}

	public DIFFICULTY difficulty = DIFFICULTY.NORMAL;	

	private FumenDataManager _fumenDataManager;

	private InputKeyJudge _inputKeyJudge;
	
	private void Start()
	{
		_fumenDataManager = FumenDataManager.Instance;
		serializeFumendata();
		
	}


	internal void serializeFumendata()
	{		
		
		var textAsset = Resources.Load ("FumenJsons/"+fileName) as TextAsset;
		var jsonText = textAsset.text;
		
		var _fumen = JsonUtility.FromJson<FumenInfo>(jsonText);
		setNoteData(_fumen);
		
		
		
	}

	private void setNoteData(FumenInfo fumenInfo)
	{
		
		//難易度に応じて全てのノーツを生成し、FumenDataManagerのMainNotesの中にしまう。
		switch (difficulty)
		{
			case DIFFICULTY.EASY:
				//イージー難易度のノーツを生成
				for (int i = 0; i < fumenInfo.easy.Count; i++)
				{
					noteGenerate(FumenDataManager.Instance.mainNotes[playerID],_fumenDataManager.timings, fumenInfo.easy[i],
						_fumenDataManager.BPM, _fumenDataManager.BEAT,0);
/*
					_fumenDataManager.mainNotes.Add(_note);
*/
				}
				//ノーマル難易度のノーツを生成
				for (int i = 0; i < fumenInfo.normal.Count; i++)
				{
					noteGenerate(FumenDataManager.Instance.mainNotes[playerID], _fumenDataManager.timings,
						fumenInfo.normal[i], _fumenDataManager.BPM, _fumenDataManager.BEAT,0.3f);
/*
					_fumenDataManager.mainNotes.Add(_note);
*/
				}

				break;
			
			case DIFFICULTY.NORMAL:
				//ノーマル難易度のノーツを生成
				for (int i = 0; i < fumenInfo.normal.Count; i++)
				{
					noteGenerate(FumenDataManager.Instance.mainNotes[playerID], _fumenDataManager.timings,
						fumenInfo.normal[i], _fumenDataManager.BPM, _fumenDataManager.BEAT,0);
/*
					_fumenDataManager.mainNotes.Add(_note);
*/
				}
				//ハード難易度のノーツを生成
				for (int i = 0; i < fumenInfo.hard.Count; i++)
				{
					noteGenerate(FumenDataManager.Instance.mainNotes[playerID], _fumenDataManager.timings,
						fumenInfo.hard[i], _fumenDataManager.BPM, _fumenDataManager.BEAT,0.3f);
/*
					_fumenDataManager.mainNotes.Add(_note);
*/
				}

				break;
			
			case DIFFICULTY.HARD:
				
				//ハード難易度のノーツを生成
				for (int i = 0; i < fumenInfo.hard.Count; i++)
				{
					noteGenerate(FumenDataManager.Instance.mainNotes[playerID], _fumenDataManager.timings,
						fumenInfo.hard[i], _fumenDataManager.BPM, _fumenDataManager.BEAT,0);
/*
					_fumenDataManager.mainNotes.Add(_note);
*/
				}
				
				//エクストラ難易度のノーツを生成
				for (int i = 0; i < fumenInfo.extra.Count; i++)
				{
					noteGenerate(FumenDataManager.Instance.moreDifficultNotes[playerID], _fumenDataManager.moreDifficultTimings,
						fumenInfo.extra[i], _fumenDataManager.BPM, _fumenDataManager.BEAT,0.3f);
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
	private void noteGenerate(List<NoteObject> targetNotesList ,
		List<FumenDataManager.NoteTimingInfomation>[,] targetTimingList,
		NotesInfo notesInfo, float _bpm, float _beat , float _ZoffSet)
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

		var _noteObject = noteGameObject.GetComponent<NoteObject>();
		
		if (_noteObject == null)
		{
			Debug.LogError("ノーツのオブジェクトにnoteObjectのスクリプトがアタッチされていませんでした");
			return;
		}
		
		//一小節あたりの長さ(単位：フレーム)
		//60 秒/_bpm (拍)で 1拍 あたり何秒なのかを算出。
		//これに60をかけて1フレームあたり何秒なのかを算出する。
		//これにbeat(拍子数)をかけることで一小節あたりのフレーム数を計算する。
		var measureLength = (60 / _bpm) * _beat;

		//一小節あたりのフレーム数がわかれば何フレーム目で到達するノーツなのかを算出できる。
		var reachFrame = measureLength * ((float) notesInfo.measure - 1) +
		                  measureLength * ((float) notesInfo.position / (float) notesInfo.split);

		//laneの長さ
		var _laneLength = FumenDataManager.Instance.laneLength;

		spawnPos = new Vector3(lane + (playerID * 20), reachFrame * _laneLength * _fumenDataManager.highSpeed, 0+_ZoffSet);
		
		//ノーツ本体のスクリプトに値を格納
		_noteObject.setNoteObject(notesInfo.type, lane, endNoteIndex, notesInfo.option, reachFrame);
		
		//生成
		var spawnedObject = Instantiate(noteGameObject, spawnPos, Quaternion.identity);
		spawnedObject.transform.parent = GameObject.Find("Notes").transform;
/*
		Debug.Log(noteGameObject.name);
*/
		
/*
		Debug.Log(FumenDataManager.Instance.mainNotes[0].Count);
*/
		targetNotesList.Add(spawnedObject.GetComponent<NoteObject>());
		
		//タイミング情報だけを格納して扱いやすくする
		if (lane >0)
		{
			targetTimingList[playerID, lane - 1]
				.Add(new FumenDataManager.NoteTimingInfomation(notesInfo.type, reachFrame));
		}

		if (notesInfo.type == 5)
		{
			Debug.Log("OTOFUDA NOTE " + spawnedObject.transform.position);
		}
		
		
		//endノーツが含まれていた場合、さらに生成してmainNotesの中につっこむ
		for (int i = 0; i < notesInfo.end.Count; i++)
		{	
			var spawnX = spawnPos.x;

			var spawnZ = 0f;
				
			//終点ノーツの生成
			noteGenerate(targetNotesList, targetTimingList, notesInfo.end[i], _bpm, _beat, _ZoffSet);
			//longNoteの終点ノーツを追加した直後なのでもっとも後ろのIndexが終点ノーツを格納したindex
			endNoteIndex = FumenDataManager.Instance.mainNotes[playerID].Count - 1;
			
			//終点情報を更新
			FumenDataManager.Instance.mainNotes[playerID][endNoteIndex-1]._endNotesNum = endNoteIndex;
				
			//終点座標からロングノーツのラインの生成座標を計算する。
			var spawnY =
				(FumenDataManager.Instance.mainNotes[playerID][endNoteIndex].reachFrame * _laneLength *
				 _fumenDataManager.highSpeed + spawnPos.y) / 2;
			
			var extend = FumenDataManager.Instance.mainNotes[playerID][endNoteIndex].reachFrame * _laneLength *
			             _fumenDataManager.highSpeed - spawnPos.y;
			
			GameObject longLine = (GameObject) Resources.Load("NoteObjects/Prefabs/testLongNote");


			var longLinePos = new Vector3(spawnX, spawnY, spawnZ);
			var scale = new Vector3(0.1f, 0.018f * extend * 9, 1);
/*
			Debug.Log(extend);
*/
			longLine.transform.localScale = scale;
			
			var spawnedLongObject = Instantiate(longLine, longLinePos, Quaternion.identity);
			spawnedLongObject.transform.parent = GameObject.Find("Notes/").transform;
			spawnedLongObject.transform.parent =
				FumenDataManager.Instance.mainNotes[playerID][endNoteIndex].gameObject.transform;

		}

	}
	



	[ContextMenu("test")]
	private void test()
	{
		var textAsset = Resources.Load ("FumenJsons/"+fileName) as TextAsset;
		var jsonText = textAsset.text;
		
		var fumenRaku = JsonUtility.FromJson<FumenInfo>(jsonText);
		Debug.Log(fumenRaku.easy[0].end[0].type); //114514
	}
}
