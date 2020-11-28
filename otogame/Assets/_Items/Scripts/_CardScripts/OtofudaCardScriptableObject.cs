using System.Collections;
using OtoFuda.Card;
using shigeno_EditorUtility;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace OtoFuda.Card
{
    [CreateAssetMenu]
    public class OtofudaCardScriptableObject : ScriptableObject
    {
        [CustomLabel("カード名")] public string cardName;
        [Header("カードの説明")] [Multiline(5)] public string cardExplain;
        [CustomLabel("月の種類")] public OtofudaMonthType monthType;
        [CustomLabel("点数の種類")] public OtofudaPointType pointType;
        [CustomLabel("カードのSprite")] public Sprite cardPicture;

        //この音札に設定されている効果
        public OtofudaCardEffectBase otofudaEffect;

        private int playerNumber;

        //バリアもちかどうか
        public bool isHaveBarrier;


        /*public void SetSprite(int playerID, int handIndex)
        {
            PlayerManager.Instance.players[playerID].SetSprites(cardPicture, handIndex);
        }*/

        /*public void SetNone(int playerID, int handIndex)
        {
            PlayerManager.Instance.players[playerID]
                .SetSprites(PlayerManager.Instance.otofudaNone.cardPicture, handIndex);
        }*/


        /*/// <summary>
        /// 音札の効果の実行。
        /// 普通、ノーツを叩いた瞬間に実行される。
        /// </summary>
        public void EffectActivate(int playerID, int handIndex)
        {
            if (otofudaEffect == null)
            {
                Debug.Log("EFFECT IS NULL!");
                return;
            }

/*
            Debug.Log(otofudaEffect.name);
#1#
            otofudaEffect.applyHandEffect(playerID, handIndex);

            Debug.Log("EFFECT ACTIVATE!");
        }*/

        //resourceフォルダからカードを使用or破棄した時のエフェクトを参照してInstantitateする。
/*        internal void destroyCard()
        {
            GameObject cardDestroyEffect = (GameObject)Resources.Load ("VFX/CardDestroyEffect");
            Instantiate(cardDestroyEffect, transform.position,Quaternion.identity);
            Destroy(cardDestroyEffect,2.0f);
        }*/
    }

    /*
    public class DeckController
    {
        [SerializeField]
        private OtofudaCard
    }*/
}