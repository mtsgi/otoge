using System.Collections;
using System.Collections.Generic;
using OtoFuda.Player;
using UnityEngine;

namespace OtoFuda.Card
{
    public class OtofudaCardEffectBase : MonoBehaviour
    {
        private OtofudaHandEffectType _handEffectCheck(OtofudaCard[] otofudaCards)
        {
            return 0;
        }
        
        public PlayerManager.Player[] _players;

        internal PlayerManager.Player _targetPlayer;
        internal int playerID;
        internal int handIndex;
        
        
        [SerializeField] private EffectTargetType _targetType;

        internal void applyHandEffect(int usedPlayerNumber ,int _handIndex)
        {
            _players = PlayerManager.Instance._players;
            playerID = usedPlayerNumber;

            //特殊効果を実行。
            if (_targetType == EffectTargetType.MYSELF)
            {
                _targetPlayer = _players[usedPlayerNumber];
            }
            //特殊効果を実行。
            else if (_targetType == EffectTargetType.OPPONENT)
            {
                //効果を発動してない方のPlayerのインデックスを引っ張ってくる。
                for (int i = 0; i < _players.Length; i++)
                {
                    if (i != usedPlayerNumber)
                    {
                        _targetPlayer = _players[i];
                    }
                }
            }

            handIndex = _handIndex;
            handEffect();
        }

        //こっちを継承して効果を作成する。
        //applyHandEffectがHandEffectCheckerから実行される。
        //_targetPlayerに対して何か操作をしてあげれば良い
        public virtual void handEffect()
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

