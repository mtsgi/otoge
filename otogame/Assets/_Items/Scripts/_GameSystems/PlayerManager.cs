using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using OtoFuda.Card;
using OtoFuda.player;
using OtoFuda.Player;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public enum PlayerFumenState
{
	MORE_EASY = 0,
	DEFAULT = 1,
	MORE_DIFFICULT = 2
}

public class PlayerManager : SingletonMonoBehaviour<PlayerManager>
{

	internal bool[] playerEffectStandby = new bool[2];
	private OtofudaCard[] otofudaCards = new OtofudaCard[2];

	private bool isRunningCoroutine;

	public OtofudaCard otofudaNone;
	

	[Serializable]
	public class Player
	{
		public string PlayerName;
		public int playerHp = 100;
		public Slider playerHPSlider;
		public int judgePoint;
		public int score;
		
		public PlayerFumenState FumenState = PlayerFumenState.DEFAULT;

		public GameObject focusObject;
		//手札
		public OtofudaCard[] playerHand; 
		//デッキ
		public List<OtofudaCard> playerDeck = new List<OtofudaCard>();
		//手札のカードの実体
		public GameObject[] playerHandCardObject =  new GameObject[5];
		internal Sprite[] handSprites = new Sprite[5];

		public ParticleSystem[] otofudaEffects;
		
		
		//ノーツ情報
		internal int[,] noteCounters = new int[3,5];
		internal int noteSimpleCount = 0;


		//選択している音札のGameObjectのインデックス
		private int focusHandCardObjectIndex = 0;
		public int selectCardIndex = 0;

		private float focusY;
		private float focusZ;

		private Color defRendererColor;
		private Color _selectColor;

		internal bool isBarrier;

		internal int activateIndex;
		
		internal void init()
		{
			defRendererColor = playerHandCardObject[3].GetComponent<SpriteRenderer>().color;
			focusY = focusObject.transform.position.y;
			focusZ = focusObject.transform.position.z;
			_selectColor = Instance.selectColor;

			for (int i = 0; i < 5; i++)
			{
				playerHandCardObject[i].GetComponent<SpriteRenderer>().sprite = playerHand[i].cardPicture;
				//Debug.Log(playerHandCardObject[i].GetComponent<SpriteRenderer>().sprite.name);
			}
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
		
		
		//現在選択しているカードを取得
		internal OtofudaCard getSelectCard()
		{
			activateIndex = selectCardIndex;
			return playerHand[selectCardIndex];
		}


		//手札枚数を返す
		internal int getActiveHandCount()
		{
			var retCount = 0;
			for (int i = 0; i < 5; i++)
			{
				if (playerHand[i].cardName == "None")
				{
					retCount++;
				}
			}

			return retCount;
		}

		//Noneの手札を探索する
		internal int getNoneHandIndex()
		{
			for (int i = 0; i < 5; i++)
			{
				if (playerHand[i].cardName == "None")
				{
					return i;
				}
			}
			
			Debug.LogError("ERROR! 手札枚数がおかしなことになっています");
			return -1;
		}

		internal void setSprites(Sprite setSprite,int targethandIndex)
		{
			playerHandCardObject[targethandIndex].GetComponent<SpriteRenderer>().sprite = setSprite;
			otofudaEffects[targethandIndex].Play();

		}
		
		
		
/*		internal void setCard(int id)
		{
			//Debug.Log(playerHand[selectCardIndex].cardName);
			playerHand[selectCardIndex].effectActivate(id, selectCardIndex);
		}*/
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
		
		for (int i = 0; i < 2; i++)
		{
			OnPlayerFocusCardChange(i, PlayerSelectState.CENTER);
			OnPlayerSelectCardChange(i, PlayerSelectState.CENTER);
		}
	}

	private void Update()
	{
//		Debug.Log(_players[0].selectCardIndex);

		Debug.Log("Player1:" + playerEffectStandby[0] + "____" + "Player2:" + playerEffectStandby[1]);
		if (playerEffectStandby[0] && playerEffectStandby[1])
		{
			//相手効果がバリアもちでなければ効果を実行。
			for (int i = 0; i < 2; i++)
			{
				if (!otofudaCards[1 - i].isHaveBarrier || otofudaCards[i].otofudaEffect._targetType != EffectTargetType.OPPONENT )
				{
					otofudaCards[i].effectActivate(i, _players[i].activateIndex);
					Debug.Log("EFFECT ACTIVATE!");
				}
			}
			
			playerEffectStandby[0] = false;
			playerEffectStandby[1] = false;
			Debug.Log("RUN");
		}
	}

	private void OnEnable()
	{
		LeapOtoFudaSelector.OnPlayerFocusCardChange += OnPlayerFocusCardChange;
		LeapOtoFudaSelector.OnPlayerSelectCardChange += OnPlayerSelectCardChange;
		
		//音札を使った時のアクション
		PlayerOtofudaMovement.OnOtofudaUse += OnUseOtoFudaCard;
	}
	

	//選択してるカードが変更されたときのイベントを定義
	private void OnPlayerFocusCardChange(int _playerID, PlayerSelectState _selectState)
	{
		
/*		var targetPlayer = _players[_playerID];
		targetPlayer.playerHandCardObject[targetPlayer.selectHandCardObjectIndex].GetComponent<Renderer>().material.color = Color.white;
		_players[_playerID].selectHandCardObjectIndex = (int) _selectState;
		targetPlayer.playerHandCardObject[(int)_selectState].GetComponent<Renderer>().material.color = Color.red;*/

		_players[_playerID].focusCard((int) _selectState);
//		Debug.Log("focusCard is "+ _selectState);
		
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
	
	
	//カードの効果を実行待ち状態にする
	private void OnUseOtoFudaCard(int _playerID, bool isPerfect)
	{
//		Debug.Log(_playerID);
		//音札ノーツを見逃してなければ効果を登録する。
		if (isPerfect)
		{
			otofudaCards[_playerID] = _players[_playerID].getSelectCard();
			
			Debug.Log(_players[_playerID].getSelectCard());
		}
		else
		{
//			Debug.Log("OTOFUDA MISS");
			otofudaCards[_playerID] = otofudaNone;
//			Debug.Log("NONE");
		}

		//Debug.Log(otofudaCards[_playerID]);
		playerEffectStandby[_playerID] = true;
	}




}
