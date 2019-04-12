using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OtoFuda.Card
{
    //音札の種別を管理する列挙型。
    //属性十二種と強度四段階で区別する。
    /// <summary>
    /// 音札の属性
    /// </summary>
    enum OtofudaFlowerType
    {
        MATSU,
        UME,
        SAKURA,
        FUJI,
        AYAME,
        BOTAN,
        HAGI,
        SUSUKI,
        KIKU,
        MOMIJI,
        YANAGI,
        KIRI,
    }
    
    /// <summary>
    /// 音札の属性情報を月で表したもの
    /// </summary>
    enum OtofudaMonthType
    {
        January,
        FEBRUARY,
        MARCH,
        APRIL,
        MAY,
        JUNE,
        JULY,
        AUGUST,
        SEPTEMBER,
        OCTOBER,
        NOVEMBER,
        DECEMBER,
    }

    /// <summary>
    /// 音札の強度
    /// 数値はコスト…とか？
    /// </summary>
    enum OtofudaPointType
    {
        SHINE = 20,
        SEED = 10,
        STRIP = 5,
        KASS = 1,
    }
    
    /// <summary>
    /// 手札の役の種類
    /// </summary>
    enum OtofudaHandEffectType
    {
        NONE,
        TANZAKU,
        TANE,
        AKATAN,
        AOTAN,
        INOSHIKACHOU,
        SANKOU,
        SHIKOU,
        AMESHIKOU,
        GOKOU,
    }


    public class OtofudaType : MonoBehaviour
    {
        internal OtofudaMonthType GetOtofudaMonthFromFlower(OtofudaFlowerType flowerType)
        {
            return (OtofudaMonthType) Enum.ToObject(typeof(OtofudaMonthType), (int) flowerType);
        }
        
        internal OtofudaFlowerType GetOtofudaFlowerFromMonth(OtofudaMonthType monthType)
        {
            return (OtofudaFlowerType) Enum.ToObject(typeof(OtofudaFlowerType), (int) monthType);
        }
        
        [ContextMenu("test")]
        private void test()
        {
            Debug.Log(GetOtofudaMonthFromFlower(OtofudaFlowerType.SAKURA));
            Debug.Log(GetOtofudaFlowerFromMonth(OtofudaMonthType.NOVEMBER));
        }
    }
}

