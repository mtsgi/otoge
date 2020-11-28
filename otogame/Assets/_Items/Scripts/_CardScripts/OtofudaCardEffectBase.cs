using System.Collections;
using System.Collections.Generic;
using OtoFuda.Player;
using UnityEngine;

namespace OtoFuda.Card
{
    public class OtofudaCardEffectBase : MonoBehaviour
    {
        private OtofudaHandEffectType _handEffectCheck(OtofudaCardScriptableObject[] otofudaCards)
        {
            return 0;
        }

        protected PlayerManager.Player[] _players;

        protected PlayerManager.Player _targetPlayer;
        protected int _handIndex = -1;
        protected OtofudaDeckController _deckController;

        internal int playerID;


        public EffectTargetType _targetType;

        internal void ApplyHandEffect(int playerIndex, int handIndex,
            OtofudaDeckController deck)
        {
            _deckController = deck;
            _handIndex = handIndex;
            
            _players = PlayerManager.Instance.players;

            //効果対象の種類によってTargetPlayerを変更する
            if (_targetType == EffectTargetType.Myself)
            {
                _targetPlayer = _players[playerIndex];
            }
            else if (_targetType == EffectTargetType.Opponent)
            {
                //効果を発動してない方のPlayerのインデックスを引っ張ってくる。
                for (int i = 0; i < _players.Length; i++)
                {
                    if (i != playerIndex)
                    {
                        _targetPlayer = _players[i];
                    }
                }
            }

            HandEffect();
        }

        //こっちを継承して効果を作成する。
        //applyHandEffectがHandEffectCheckerから実行される。
        //_targetPlayerに対して何か操作をしてあげれば良い
        public virtual void HandEffect()
        {
/*            //いったん使用した手札の情報をNoneの音札に置き換える
            _targetPlayer.playerHand[handIndex] = PlayerManager.Instance.otofudaNone;
        
            //まずワンドロー
            if (_targetPlayer.playerDeck.Count != 0)
            {
                _targetPlayer.playerHand[handIndex] = _targetPlayer.playerDeck[0];
                _targetPlayer.playerDeck.RemoveAt(0);
            }*/
        }
    }
}