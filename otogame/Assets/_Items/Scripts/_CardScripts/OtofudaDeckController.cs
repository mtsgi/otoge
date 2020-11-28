using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OtoFuda.Card
{
    public class OtofudaDeckController : MonoBehaviour
    {
        public Color defaultColor = Color.white;

        //手札として表示されているカードのゲームオブジェクト
        public List<OtofudaCardObject> otofudaHandObjects;
        private Stack<OtofudaCard> _otofudaHandCards = new Stack<OtofudaCard>();

        //デッキに存在しているカード
        private Stack<OtofudaCard> _otofudaDeckCards = new Stack<OtofudaCard>();
        private int _playerIndex = -1;

        private OtofudaCard noneCard;

        [Header("テスト用")] public OtofudaCardSetList CardSetList;

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.F12))
            {
                for (int i = 0; i < _otofudaDeckCards.Count; i++)
                {
                    Debug.Log(_otofudaDeckCards.ToArray()[i].cardName);
                }
            }
        }

        public void Init(int playerIndex)
        {
            _playerIndex = playerIndex;

            noneCard = new OtofudaCard(PlayerManager.Instance.otofudaNone, this);

            //todo デッキをAPIからひろってきたりする？

            DeckInitialize();

            //初期手札の三枚をドロー
            for (int i = 0; i < 3; i++)
            {
                Draw();
            }
        }

        private void DeckInitialize()
        {
            _otofudaDeckCards = new Stack<OtofudaCard>();
            if (CardSetList != null)
            {
                for (int i = 0; i < CardSetList.otofudaCardScriptableObjects.Count; i++)
                {
                    var cardInfo = CardSetList.otofudaCardScriptableObjects[i];
                    var card = new OtofudaCard(cardInfo, this);
                    _otofudaDeckCards.Push(card);
                }
            }
        }

        public void Draw()
        {
            if (_otofudaDeckCards != null && _otofudaDeckCards.Count != 0)
            {
                //デッキのスタックから一番上を抜く
                var drawCard = _otofudaDeckCards.Pop();

                //手札枚数が5枚未満(最大でなければ)手札のスタックに追加する
                if (_otofudaHandCards.Count < 5)
                {
                    _otofudaHandCards.Push(drawCard);
                    //Otofudaが登録されていない手札オブジェクトを探して手札を更新する
                    var emptyHandObject = otofudaHandObjects.FirstOrDefault(x =>
                        x.Otofuda == null || x.Otofuda.monthType == OtofudaMonthType.NONE);
                    if (emptyHandObject != null)
                    {
                        emptyHandObject.SetCard(drawCard);
                    }
                }
            }
        }

        public void DiscardHand(int targetHandIndex)
        {
            //Noneを設定する
            otofudaHandObjects[targetHandIndex].Otofuda = null;
            otofudaHandObjects[targetHandIndex].SetCard(noneCard);

            _otofudaHandCards.Pop();
        }

        public void UseCard(int selectIndex)
        {
            var card = otofudaHandObjects[selectIndex].Otofuda;
            if (card != null && card.monthType != OtofudaMonthType.NONE)
            {
                card.EffectActivate(_playerIndex, selectIndex);
            }

            //カードを使用したらドローする
            Draw();
        }

        [ContextMenu("Assign OtofudaObject")]
        private void AssignOtofudaObjects()
        {
            otofudaHandObjects = new List<OtofudaCardObject>();
            var otofudaObjects = GetComponentsInChildren<OtofudaCardObject>();
            for (int i = 0; i < otofudaObjects.Length; i++)
            {
                otofudaHandObjects.Add(otofudaObjects[i]);
            }
        }
    }
}