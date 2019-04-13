using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OtoFuda.Card
{
    //手札効果を辞書登録しておくクラス。
    public class OtofudaHandEffect : SingletonMonoBehaviour<OtofudaHandEffect> {

        
        internal Dictionary<string,OtofudaHandEffectBase> effectDictionary;
        
        
       
        
    }
   
}