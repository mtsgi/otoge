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

        [SerializeField] internal OtofudaFlowerType _flowerType;
        [SerializeField] internal OtofudaPointType _pointType;

        //この音札に設定されている効果
        [SerializeField] internal OtofudaCardEffectBase otofudaEffect;

        private void OnEnable()
        {
            
        }
    }
}

