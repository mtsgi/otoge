using UnityEngine;

namespace OtoFuda.Card
{
    public class OtofudaHandEffectBase : MonoBehaviour
    {
        private PlayerManager.Player[] _players;

        private void Start()
        {
            //Player情報を引っ張ってくる。
            _players = PlayerManager.Instance._players;
        }

        public virtual void applyHandEffect()
        {
            
        }
    }
}
