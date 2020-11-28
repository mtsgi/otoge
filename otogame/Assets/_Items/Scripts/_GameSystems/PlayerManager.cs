using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using OtoFuda.Card;
using OtoFuda.player;
using OtoFuda.Player;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public enum PlayerFumenState
{
    MoreEasy = 0,
    Default = 1,
    MoreDifficult = 2,
    DefaultBeatLine = 3,
    MoreDifficultBeatLine = 4,
    MoreEasyBeatLine = 5,
    End,
}

public class PlayerManager : SingletonMonoBehaviour<PlayerManager>
{
    internal bool[] playerEffectStandby = new bool[2];
    /*private OtofudaCardObject[] otofudaCards = new OtofudaCardObject[2];*/

    private int[] playerHpBuffer = new int[2] {100, 100};
    private bool isRunningCoroutine;

    public OtofudaCardScriptableObject otofudaNone;


    [Serializable]
    public class Player
    {
        public string PlayerName = "Guest";
        public int playerHp = 100;
        public int playerMaxHp = 100;
        public float hiSpeed = 8.0f;
        public Slider playerHPSlider;
        public int judgePoint;
        public int score;

        public PlayerFumenState FumenState = PlayerFumenState.Default;

        public GameObject focusObject;

        public OtofudaDeckController deckController;

        /*//手札
        public OtofudaCard[] playerHand;

        //デッキ
        public List<OtofudaCard> playerDeck = new List<OtofudaCard>();

        //手札のカードの実体
        public OtofudaCardObject[] playerHandCardObject = new OtofudaCardObject[5];
        internal Sprite[] handSprites = new Sprite[5];*/

        public ParticleSystem[] otofudaEffects;


        //選択している音札のGameObjectのインデックス
        private int focusHandCardObjectIndex = 0;
        public int selectCardIndex = 0;

        private float focusY;
        private float focusZ;

        private Color defRendererColor;
        private Color _selectColor;

        internal bool isBarrier;

        internal int activateIndex;

        internal void Init(int playerIndex)
        {
/*			PlayerName = PlayerInformationManager.Instance.name[playerIndex];*/
            /*PlayerName =*/
            defRendererColor = deckController.defaultColor;
            deckController.Init(playerIndex);

            focusY = focusObject.transform.position.y;
            focusZ = focusObject.transform.position.z;
            _selectColor = Instance.selectColor;
            playerHPSlider.maxValue = playerMaxHp;
        }

        internal void FocusCard(int selectStatenum)
        {
            //focusとselectが同じだった場合は色の変更はしない(てｓｔ用)
            if (focusHandCardObjectIndex != selectCardIndex)
            {
                deckController.otofudaHandObjects[focusHandCardObjectIndex].GetComponent<SpriteRenderer>().color =
                    defRendererColor;
            }

            focusHandCardObjectIndex = (int) selectStatenum;
            var nextFocusPosition = deckController.otofudaHandObjects[selectStatenum].transform.position;
            nextFocusPosition.y = focusY;
            nextFocusPosition.z = focusZ;

            focusObject.transform.position = nextFocusPosition;

/*			playerHandCardObject[selectStatenum].GetComponent<Renderer>().material.color = Color.red;*/
        }

        internal void SelectCard()
        {
            deckController.otofudaHandObjects[selectCardIndex].GetComponent<SpriteRenderer>().color = defRendererColor;
            deckController.otofudaHandObjects[focusHandCardObjectIndex].GetComponent<SpriteRenderer>().color =
                _selectColor;
            selectCardIndex = focusHandCardObjectIndex;
        }


        //現在選択しているカードを取得
        /*internal OtofudaCardObject GetSelectCard()
        {
            activateIndex = selectCardIndex;
            return deckController.otofudaHandObjects[selectCardIndex];
        }*/

        public void UseSelectedCard()
        {
            deckController.UseCard(selectCardIndex);
        }


        //手札枚数を返す
        internal int GetActiveHandCount()
        {
            var retCount = 0;
            /*for (int i = 0; i < 5; i++)
            {
                if (playerHand[i].cardName == "None")
                {
                    retCount++;
                }
            }*/

            return retCount;
        }

        //Noneの手札を探索する
        internal int GetNoneHandIndex()
        {
            for (int i = 0; i < 5; i++)
            {
                /*if (playerHand[i].cardName == "None")
                {
                    return i;
                }*/
            }

            Debug.LogError("ERROR! 手札枚数がおかしなことになっています");
            return -1;
        }


/*		internal void setCard(int id)
		{
			//Debug.Log(playerHand[selectCardIndex].cardName);
			playerHand[selectCardIndex].effectActivate(id, selectCardIndex);
		}*/
    }

    public Color selectColor;

    public Player[] players = new Player[]
    {
        new Player() {PlayerName = "player1", judgePoint = 0, score = 0},
        new Player() {PlayerName = "player2", judgePoint = 0, score = 0},
    };

    private void Start()
    {
        for (int i = 0; i < players.Length; i++)
        {
            players[i].Init(i);
        }

        for (int i = 0; i < players.Length; i++)
        {
            OnPlayerFocusCardChange(i, PlayerSelectState.CENTER);
            OnPlayerSelectCardChange(i, PlayerSelectState.CENTER);

            var playerId = i;
            this.ObserveEveryValueChanged(x => players[playerId].playerHp)
                .Where(x => x >= 0)
                .Subscribe(_ => OnPlayerHpChanged(playerId));
        }
    }

    //現在の二人のHpを送信する
    private void OnPlayerHpChanged(int playerId)
    {
        playerHpBuffer[playerId] = players[playerId].playerHp;
//		Debug.Log(_players[playerId].playerHp);
        OtofudaSerialPortManager.Instance.SendPlayerHp(playerHpBuffer);
    }

    private void Update()
    {
//		Debug.Log(_players[0].selectCardIndex);

//		Debug.Log("Player1:" + playerEffectStandby[0] + "____" + "Player2:" + playerEffectStandby[1]);
/*		if (playerEffectStandby[0] && playerEffectStandby[1])
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
		}*/
    }

    private void OnEnable()
    {
        LeapOtoFudaSelector.OnPlayerFocusCardChange += OnPlayerFocusCardChange;
        LeapOtoFudaSelector.OnPlayerSelectCardChange += OnPlayerSelectCardChange;

/*		//音札を使った時のアクション
		PlayerOtofudaMovement.OnOtofudaUse += OnUseOtoFudaCard;*/
    }


    //選択してるカードが変更されたときのイベントを定義
    private void OnPlayerFocusCardChange(int _playerID, PlayerSelectState _selectState)
    {
/*		var targetPlayer = _players[_playerID];
		targetPlayer.playerHandCardObject[targetPlayer.selectHandCardObjectIndex].GetComponent<Renderer>().material.color = Color.white;
		_players[_playerID].selectHandCardObjectIndex = (int) _selectState;
		targetPlayer.playerHandCardObject[(int)_selectState].GetComponent<Renderer>().material.color = Color.red;*/

        players[_playerID].FocusCard((int) _selectState);
//		Debug.Log("focusCard is "+ _selectState);
    }

    //選択されているカードが選択された時のイベントを定義
    private void OnPlayerSelectCardChange(int _playerID, PlayerSelectState _selectState)
    {
/*		var targetPlayer = _players[_playerID];
		targetPlayer.playerHandCardObject[targetPlayer.selectHandCardObjectIndex].GetComponent<Renderer>().material.color = Color.white;
		_players[_playerID].selectHandCardObjectIndex = (int) _selectState;
		targetPlayer.playerHandCardObject[(int)_selectState].GetComponent<Renderer>().material.color = Color.red;*/

        players[_playerID].SelectCard();
    }


    //カードの効果を実行待ち状態にする
    public void OnUseOtofudaCard(int playerID, bool isPerfect)
    {
        players[playerID].UseSelectedCard();

/*//        Debug.Log($"{playerID} use 音札 ; {_players[playerID].GetSelectCard()}");

        //音札ノーツを見逃してなければ効果を登録する。
        if (isPerfect)
        {
            otofudaCards[playerID] = players[playerID].GetSelectCard();
            otofudaCards[playerID].EffectActivate(playerID, players[playerID].activateIndex);

            Debug.Log(players[playerID].GetSelectCard());
        }
        else
        {
//			Debug.Log("OTOFUDA MISS");
            otofudaCards[playerID] = otofudaNone;
//			Debug.Log("NONE");
        }

        //Debug.Log(otofudaCards[_playerID]);
        playerEffectStandby[playerID] = true;*/
    }
}