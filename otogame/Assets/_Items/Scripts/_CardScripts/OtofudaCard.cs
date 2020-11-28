using UnityEngine;

namespace OtoFuda.Card
{
    public class OtofudaCard
    {
        public string cardName;
        public OtofudaMonthType monthType;
        public OtofudaCardEffectBase cardEffect;
        public OtofudaPointType pointType;
        public Sprite cardPicture;
        public bool isHaveBarrier;

        private OtofudaDeckController _deckController;

        public OtofudaCard(OtofudaCardScriptableObject cardScriptableObject,
            OtofudaDeckController deckController)
        {
            cardName = cardScriptableObject.cardName;
            monthType = cardScriptableObject.monthType;
            cardEffect = cardScriptableObject.otofudaEffect;
            pointType = cardScriptableObject.pointType;
            cardPicture = cardScriptableObject.cardPicture;
            isHaveBarrier = cardScriptableObject.isHaveBarrier;

            _deckController = deckController;
        }


        /// <summary>
        /// 音札の効果の実行。
        /// 普通、ノーツを叩いた瞬間に実行される。
        /// </summary>
        public void EffectActivate(int playerIndex, int handIndex)
        {
            cardEffect.ApplyHandEffect(playerIndex, handIndex, _deckController);
        }
    }
}