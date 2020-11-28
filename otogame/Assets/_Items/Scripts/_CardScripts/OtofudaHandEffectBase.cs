using OtoFuda.Player;
using UnityEngine;

namespace OtoFuda.Card
{
    public class OtofudaHandEffectBase : MonoBehaviour
    {
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
            _players = PlayerManager.Instance.players;
            
            
        }

        //手札の効果を実行する部分。
        //usePlayerNumberが効果を発動したPlayer。
        internal void applyHandEffect(int usedPlayerNumber)
        {
            //特殊効果を実行。
            if (_targetType == EffectTargetType.Myself)
            {
                _targetPlayer = _players[usedPlayerNumber];
            }
            //特殊効果を実行。
            else if (_targetType == EffectTargetType.Opponent)
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

        //こっちを継承して効果を実行する。
        //applyHandEffectがHandEffectCheckerから実行される。
        //_targetPlayerに対して何か操作をしてあげれば良い
        public virtual void handEffect()
        {
            
        }
    }
}
