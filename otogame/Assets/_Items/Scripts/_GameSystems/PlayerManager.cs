using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using OtoFuda.Card;
using OtoFuda.player;
using UnityEngine;

public class PlayerManager : SingletonMonoBehaviour<PlayerManager> {
	
	[Serializable]
	public class Player
	{
		public string PlayerName;
		public int judgePoint;
		public int score;

		public GameObject focusObject;
		//手札
		public OtofudaCard[] playerHand; 
		//デッキ
		public List<OtofudaCard> playerDeck = new List<OtofudaCard>();
		public GameObject[] playerHandCardObject =  new GameObject[5];
 

		//選択している音札のGameObjectのインデックス
		private int focusHandCardObjectIndex = 0;
		private int selectCardIndex = 0;

		private float focusY;
		private float focusZ;

		private Color defRendererColor;
		private Color _selectColor;
		
		
		internal void init()
		{
			defRendererColor = playerHandCardObject[3].GetComponent<SpriteRenderer>().color;
			focusY = focusObject.transform.position.y;
			focusZ = focusObject.transform.position.z;
			_selectColor = Instance.selectColor;
		}
		
		internal void focusCard(int selectStatenum)
		{
			//focusとselectが同じだった場合は色の変更はしない(てｓｔ用)
			if (focusHandCardObjectIndex != selectCardIndex)
			{
				playerHandCardObject[focusHandCardObjectIndex].GetComponent<SpriteRenderer>().color = defRendererColor;
			}
			focusHandCardObjectIndex = (int) selectStatenum;
			var nextFocusPosition = playerHandCardObject[selectStatenum].transform.position;
			nextFocusPosition.y = focusY;
			nextFocusPosition.z = focusZ;

			focusObject.transform.position = nextFocusPosition;
			
/*			playerHandCardObject[selectStatenum].GetComponent<Renderer>().material.color = Color.red;*/
		}

		internal void selectCard()
		{
			playerHandCardObject[selectCardIndex].GetComponent<SpriteRenderer>().color = defRendererColor;
			playerHandCardObject[focusHandCardObjectIndex].GetComponent<SpriteRenderer>().color = _selectColor;
			selectCardIndex = focusHandCardObjectIndex;
		}

		internal void useCard(int id)
		{
			Debug.Log(playerHand[selectCardIndex].cardName);

			playerHand[selectCardIndex].effectActivate(id, selectCardIndex);
		}
		
	}

	 public Color selectColor;

	[SerializeField] [Multiline(3)]
	private string _description = "プレイヤーの情報を登録しておくところ。\n開発中はここ直でいじれるようにしておくけど\n長さを2以上にしても意味ないです";
	public Player[] _players = new Player[]
	{
		new Player(){PlayerName = "player1",judgePoint = 0,score = 0},
		new Player(){PlayerName = "player2",judgePoint = 0,score = 0},
	};

	private void Start()
	{
		for (int i = 0; i < 2; i++)
		{
			_players[i].init();
		}
	}

	private void OnEnable()
	{
		LeapOtoFudaSelector.OnPlayerFocusCardChange += OnPlayerFocusCardChange;
		LeapOtoFudaSelector.OnPlayerSelectCardChange += OnPlayerSelectCardChange;
		
		//音札を使った時のアクション
		InputKeyJudge.OnUseOtoFudaCard += OnUseOtoFudaCard;
	}

	//選択してるカードが変更されたときのイベントを定義
	private void OnPlayerFocusCardChange(int _playerID, PlayerSelectState _selectState)
	{
		
/*		var targetPlayer = _players[_playerID];
		targetPlayer.playerHandCardObject[targetPlayer.selectHandCardObjectIndex].GetComponent<Renderer>().material.color = Color.white;
		_players[_playerID].selectHandCardObjectIndex = (int) _selectState;
		targetPlayer.playerHandCardObject[(int)_selectState].GetComponent<Renderer>().material.color = Color.red;*/

		_players[_playerID].focusCard((int) _selectState);
		
	}
	
	//選択されているカードが選択された時のイベントを定義
	private void OnPlayerSelectCardChange(int _playerID, PlayerSelectState _selectState)
	{
		
/*		var targetPlayer = _players[_playerID];
		targetPlayer.playerHandCardObject[targetPlayer.selectHandCardObjectIndex].GetComponent<Renderer>().material.color = Color.white;
		_players[_playerID].selectHandCardObjectIndex = (int) _selectState;
		targetPlayer.playerHandCardObject[(int)_selectState].GetComponent<Renderer>().material.color = Color.red;*/

		_players[_playerID].selectCard();

	}


	private void OnUseOtoFudaCard(int _playerID)
	{
		_players[_playerID].useCard(_playerID);
	}
}
