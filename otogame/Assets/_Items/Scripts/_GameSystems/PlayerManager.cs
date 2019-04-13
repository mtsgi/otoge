using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerManager : SingletonMonoBehaviour<PlayerManager> {

	//レーン情報や譜面情報、ライフなどを格納しておきたい。
	//Effect系スクリプトに影響を与えられるようにするため。

	
	[Serializable]
	public class Player
	{
		public string PlayerName;
		public int judgePoint;
		public int score;	
	}

	[SerializeField] [Multiline(3)]
	private string _description = "プレイヤーの情報を登録しておくところ。\n開発中はここ直でいじれるようにしておくけど\n長さを2以上にしても意味ないです";
	[SerializeField] internal Player[] _players = new Player[]
	{
		new Player(){PlayerName = "player1",judgePoint = 0,score = 0},
		new Player(){PlayerName = "player2",judgePoint = 0,score = 0},
	};


	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
