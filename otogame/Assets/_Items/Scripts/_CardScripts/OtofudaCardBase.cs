using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OtoFuda.Card
{
    /// <summary>
    /// 音札のカードクラス。
    /// これを継承してカードに効果を持たせる。
    /// </summary>
    public class OtofudaCardBase : MonoBehaviour
    {

        [SerializeField] private OtofudaFlowerType _flowerType;
        [SerializeField] private OtofudaPointType _pointType;
    
        //カードの効果を実行する関数
        internal virtual void applyEffect()
        {
        
        }
    
    }
}

