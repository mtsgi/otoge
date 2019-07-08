using System.Collections;
using System.Collections.Generic;
using shigeno_EditorUtility;
using UnityEngine;
using UnityEngine.UI;

namespace OtoFuda.Card
{
    public class OtofudaCard : MonoBehaviour
    {
        [CustomLabel("カード名")]
        [SerializeField] internal string cardName;
        [CustomLabel("月の種類")]
        [SerializeField] internal OtofudaMonthType _monthType;
        [CustomLabel("点数の種類")]
        [SerializeField] internal OtofudaPointType _pointType;
        
        //この音札に設定されている効果
        [SerializeField] internal OtofudaCardEffectBase otofudaEffect;

        private int playerNumber;

        private void Awake()
        {

        }

        private void OnEnable()
        {
        }

        
        /// <summary>
        /// 音札の効果の実行。
        /// 普通、ノーツを叩いた瞬間に実行される。
        /// </summary>
        internal void effectActivate()
        {
            otofudaEffect.applyHandEffect(playerNumber);
        }

        //resourceフォルダからカードを使用or破棄した時のエフェクトを参照してInstantitateする。
        internal void destroyCard()
        {
            GameObject cardDestroyEffect = (GameObject)Resources.Load ("VFX/CardDestroyEffect");
            Instantiate(cardDestroyEffect, transform.position,Quaternion.identity);
            Destroy(cardDestroyEffect,2.0f);
        }
    }
}

