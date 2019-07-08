using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OtoFuda.Card
{
    //ｽﾊﾟｯﾊﾟｯﾊﾟﾗｯﾊﾟｰwwwwﾊﾟｯﾊﾟｯﾊﾟｯﾊﾟｯﾊﾟﾗｯﾊﾟｰwwww ｽﾊﾟｯﾊﾟﾗｯﾊﾟｰwwwｽﾊﾟｲｱﾝﾄﾞｽﾊﾟｲwwww(・_・)ｽｯ
    //手札効果を辞書登録しておくクラス。
    public class OtofudaHandEffectManager : SingletonMonoBehaviour<OtofudaHandEffectManager> {

        //手札の効果を格納しておく辞書型
        internal Dictionary<string,OtofudaHandEffectBase> effectDictionary; 
        
        [Header("手札効果のリスト")]
        [SerializeField] private OtofudaHandEffectBase[] _handEffectList = new OtofudaHandEffectBase[Enum.GetValues(typeof(OtofudaHandEffectType)).Length -1];
                
        private void Awake()
        {
            int _handEffectLength = _handEffectList.Length;
            int _handeffectTypeLength = Enum.GetValues(typeof(OtofudaHandEffectType)).Length -1;
            //手札効果の名前の一覧と登録したエフェクト数が一致しなかった場合破棄する。
            if (_handEffectLength != _handeffectTypeLength)
            {
                Debug.LogError("handeEffectListが規定の個数ありません。");
                dispose();
            }
            //辞書の初期化
            effectDictionary = new Dictionary<string, OtofudaHandEffectBase>();
            
            for (int i = 0; i < Enum.GetValues(typeof(OtofudaHandEffectType)).Length - 1; i++)
            {
                //handeffectListがNUllであれば弾く
                if (_handEffectList[i] == null)
                {
                    return;
                }
                else
                {
                    //辞書に役名と効果のあるスクリプトを登録する。
                    effectDictionary.Add(Enum.ToObject(typeof(OtofudaHandEffectType),i+1).ToString(),_handEffectList[i]);
                    //Debug.Log(Enum.ToObject(typeof(OtofudaHandEffectType),i+1).ToString()+","+_handEffectList[i].name);
                }
            }
        }

        //終了処理。
        private void dispose()
        {
            enabled = false;
        }

        
        //手札効果のスクリプトを_handEffectListの中に自動で登録する。
        [ContextMenu("AutoSetHandEffectScripts")]
        private void autoset()
        {
            _handEffectList = new OtofudaHandEffectBase[Enum.GetValues(typeof(OtofudaHandEffectType)).Length -1];
            var handEffects = GetComponents<OtofudaHandEffectBase>();
            for (int i = 0; i < GetComponents<OtofudaHandEffectBase>().Length; i++)
            {
                _handEffectList[i] = handEffects[i];
                
                //test
                //_handEffectList[i].applyHandEffect();
            }
        }
        
        
    }
   
}