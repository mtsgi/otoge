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
        
        private PlayerManager.Player[] _players;

        private PlayerManager.Player _targetPlayer;

        [SerializeField] private EffectTargetType _targetType;
        private void Start()
        {
            if (PlayerManager.Instance == null)
            {
                Debug.LogError("<color=#FF0000>ERROR</color> on"+name+" ! PlayerManagerがScene存在していないか、非表示になってるっぽいです。");
                return;
            }
            
            //Player情報を引っ張ってくる。
            _players = PlayerManager.Instance._players;
        }

        //手札の効果を実行する部分。
        //usePlayerNumberが効果を発動したPlayer。
        internal void applyHandEffect(int usedPlayerNumber)
        {
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
            
            handEffect();
        }

        //こっちを継承して効果を作成する。
        //applyHandEffectがHandEffectCheckerから実行される。
        //_targetPlayerに対して何か操作をしてあげれば良い
        public virtual void handEffect()
        {
            
        }

    }




}

