using System.Collections;
using System.Collections.Generic;
using shigeno_EditorUtility;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace OtoFuda.Card
{
    [CreateAssetMenu]
    public class OtofudaCard : ScriptableObject
    {
        [CustomLabel("カード名")]
        [SerializeField] internal string cardName;
        [Header("カードの説明")][Multiline(5)]
        [SerializeField] internal string cardExplain;
        [CustomLabel("月の種類")]
        [SerializeField] internal OtofudaMonthType _monthType;
        [CustomLabel("点数の種類")]
        [SerializeField] internal OtofudaPointType _pointType;

        [CustomLabel("カードのSprite")] 
        [SerializeField] internal Sprite cardPicture;
        
        //この音札に設定されている効果
        public OtofudaCardEffectBase otofudaEffect;
        
        private int playerNumber;
        
        //バリアもちかどうか
        public bool isHaveBarrier;
        
        
        internal void setSprite(int _playerID, int _handIndex)
        {
            PlayerManager.Instance._players[_playerID].setSprites(cardPicture, _handIndex);
        }
        
        internal void setNone(int _playerID, int _handIndex)
        {
            PlayerManager.Instance._players[_playerID].setSprites(PlayerManager.Instance.otofudaNone.cardPicture, _handIndex);
        }
        

        /// <summary>
        /// 音札の効果の実行。
        /// 普通、ノーツを叩いた瞬間に実行される。
        /// </summary>
        public void effectActivate(int _playerID, int _handIndex)
        {
            if (otofudaEffect == null)
            {
                Debug.Log("EFFECT IS NULL!");
                return;
            }

/*
            Debug.Log(otofudaEffect.name);
*/
            otofudaEffect.applyHandEffect(_playerID, _handIndex);
            
            Debug.Log("EFFECT ACTIVATE!");

        }

        //resourceフォルダからカードを使用or破棄した時のエフェクトを参照してInstantitateする。
/*        internal void destroyCard()
        {
            GameObject cardDestroyEffect = (GameObject)Resources.Load ("VFX/CardDestroyEffect");
            Instantiate(cardDestroyEffect, transform.position,Quaternion.identity);
            Destroy(cardDestroyEffect,2.0f);
        }*/
    }
}

