using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using OtoFuda.Fumen;
using MiniJSON;
using UnityEngine;

public class JsonReadManager : SingletonMonoBehaviour<JsonReadManager>
{

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
				for (int i = 0; i < fumenInfo.easy.Count; i ++)
				{
					var _note = noteGenerate(fumenInfo.easy[i], _fumenDataManager.BPM,
						_fumenDataManager.BEAT);
					_fumenDataManager.mainNotes.Add(_note);
				}
				break;
			
			case DIFFICULTY.NORMAL:
				for (int i = 0; i < fumenInfo.normal.Count; i ++)
				{
					var _note = noteGenerate(fumenInfo.normal[i], _fumenDataManager.BPM,
						_fumenDataManager.BEAT);
					_fumenDataManager.mainNotes.Add(_note);
				}
				break;
			
			case DIFFICULTY.HARD:
				for (int i = 0; i < fumenInfo.hard.Count; i ++)
				{
					var _note = noteGenerate(fumenInfo.hard[i], _fumenDataManager.BPM,
						_fumenDataManager.BEAT);
					_fumenDataManager.mainNotes.Add(_note);
				}
				break;
				
		}
		
	}


	private NoteObject noteGenerate(NotesInfo notesInfo, float _bpm, float _beat)
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
				noteGameObject = (GameObject) Resources.Load("NoteObjects/Prefabs/OtofuadNote");
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
			return null;
		}
		

		
		//endノーツが含まれていた場合、さらに生成してmainNotesの中につっこむ
		if (notesInfo.type == 2)
		{
			for (int i = 0; i < notesInfo.end.Count; i++)
			{
				var longEndNote = noteGenerate(notesInfo.end[i], _bpm, _beat); 
				FumenDataManager.Instance.mainNotes.Add(longEndNote);
				endNoteIndex = FumenDataManager.Instance.mainNotes.Count - 1;
			}
		}
		
		


		//一小節あたりの長さ(単位：フレーム)
		//60 秒/_bpm (拍)で 1拍 あたり何秒なのかを算出。
		//これに60をかけて1フレームあたり何秒なのかを算出する。
		//これにbeat(拍子数)をかけることで一小節あたりのフレーム数を計算する。
		var measureLength = (3600 / _bpm) * _beat;

		//一小節あたりのフレーム数がわかれば何フレーム目で到達するノーツなのかを算出できる。
		var reachFrame = measureLength * ((float) notesInfo.measure - 1) +
		                  measureLength * ((float) notesInfo.position / (float) notesInfo.split);

		//laneの長さ
		var _laneLength = FumenDataManager.Instance.laneLength;
		
		spawnPos = new Vector3(lane, reachFrame * _laneLength, 0);
		
		//ノーツ本体のスクリプトに値を格納
		_noteObject.setNoteObject(notesInfo.type, lane, endNoteIndex, notesInfo.option, reachFrame);
		
		//生成
		Instantiate(noteGameObject, spawnPos, Quaternion.identity);

		return noteGameObject.GetComponent<NoteObject>();
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
