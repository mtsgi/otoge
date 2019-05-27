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
		
		//難易度に応じて全てのノーツを生成する。
		switch (difficulty)
		{
			case DIFFICULTY.EASY:
				for (int i = 0; i < fumenInfo.easy.Count; i ++)
				{
					_fumenDataManager.mainNotes.Add(new Note(fumenInfo.easy[i], _fumenDataManager.BPM,
						_fumenDataManager.BEAT));
				}
				break;
			
			case DIFFICULTY.NORMAL:
				for (int i = 0; i < fumenInfo.normal.Count; i ++)
				{
					_fumenDataManager.mainNotes.Add(new Note(fumenInfo.normal[i], _fumenDataManager.BPM,
						_fumenDataManager.BEAT));
				}
				break;
			
			case DIFFICULTY.HARD:
				for (int i = 0; i < fumenInfo.hard.Count; i ++)
				{
					_fumenDataManager.mainNotes.Add(new Note(fumenInfo.hard[i], _fumenDataManager.BPM,
						_fumenDataManager.BEAT));
				}
				break;
				
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
