using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using OtoFuda.player;
using UnityEngine;

public class PlayerManager : SingletonMonoBehaviour<PlayerManager> {

	[Serializable]
	public class Player
	{
		public string PlayerName;
		public int judgePoint;
		public int score;

		public GameObject[] playerHandCardObject =  new GameObject[5];

		//選択している音札のGameObjectのインデックス
		internal int selectHandCardObjectIndex = 0;
		
	}

	[SerializeField] [Multiline(3)]
	private string _description = "プレイヤーの情報を登録しておくところ。\n開発中はここ直でいじれるようにしておくけど\n長さを2以上にしても意味ないです";
	[SerializeField] private Player[] _players = new Player[]
	{
		new Player(){PlayerName = "player1",judgePoint = 0,score = 0},
		new Player(){PlayerName = "player2",judgePoint = 0,score = 0},
	};

	private void OnEnable()
	{
		LeapOtoFudaSelector.OnPlayerSelectCardChange += OnPlayerSelectCardChange;
	}

	//選択してるカードが変更されたときのイベントを定義
	private void OnPlayerSelectCardChange(int _playerID, PlayerSelectState _selectState)
	{
		var targetPlayer = _players[_playerID];
		targetPlayer.playerHandCardObject[targetPlayer.selectHandCardObjectIndex].GetComponent<Renderer>().material.color = Color.white;
		_players[_playerID].selectHandCardObjectIndex = (int) _selectState;
		targetPlayer.playerHandCardObject[(int)_selectState].GetComponent<Renderer>().material.color = Color.red;

	}
}
